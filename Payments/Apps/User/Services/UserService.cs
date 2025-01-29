using ErrorOr;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Payments.Apps.Mail.Interfaces;
using Payments.Apps.Mail.Services;
using Payments.Apps.Org.Models;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Models;
using Payments.DTOs;
using System.Text.RegularExpressions;
using static Payments.Errors;

namespace Payments.Apps.User.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<UserModel> _userCollection;
        private readonly IEmailSender _emailSender;

        public UserService(IMongoClient mongoClient, IEmailSender emailSender)
        {
            var database = mongoClient.GetDatabase("PaymentsDB");
            _userCollection = database.GetCollection<UserModel>("Users");
            _emailSender = emailSender;
        }

        public async Task<ErrorOr<UserModel>> LoginWithEmail(string email, string password)
        {
            var user = await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null || !PasswordHelper.VerifyPassword(password, user.Pnr))
            {
                return Error.Validation("InvalidCredentials", "Invalid email or password.");
            }
            return user;
        }

        public async Task<ErrorOr<UserModel>> LoginWithMobile(string phone, string password)
        {
            var user = await _userCollection.Find(u => u.Phone == phone).FirstOrDefaultAsync();
            if (user == null || !PasswordHelper.VerifyPassword(password, user.Pnr))
            {
                return Error.Validation("InvalidCredentials", "Invalid phone or password.");
            }
            return user;
        }

        public async Task<ErrorOr<UserModel>> RegisterWithEmail(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return Errors.User.InvalidEmail;
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(registerDto.Email))
            {
                return Errors.User.InvalidEmail;
            }

            var emailExists = await _userCollection.Find(u => u.Email == registerDto.Email).FirstOrDefaultAsync();
            if (emailExists != null)
            {
                return Errors.User.EmailAlreadyExists;
            }

            if (!string.IsNullOrWhiteSpace(registerDto.Phone))
            {
                var phoneExists = await _userCollection.Find(u => u.Phone == registerDto.Phone).FirstOrDefaultAsync();
                if (phoneExists != null)
                {
                    return Errors.User.PhoneAlreadyExists;
                }
            }

            var maxVisualId = await _userCollection
                .Find(_ => true)
                .SortByDescending(u => u.VisualId)
                .Limit(1)
                .Project(u => u.VisualId)
                .FirstOrDefaultAsync();

            var newUser = new UserModel
            {
                VisualId = maxVisualId + 1,
                Email = registerDto.Email,
                Phone = registerDto.Phone ?? null,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Pnr = PasswordHelper.HashPassword(registerDto.Password),
                Roles = new List<string> { "User" },
                Country = registerDto.Country,
                Status = "Unverified",
                CreatedAt = DateTime.UtcNow
            };

            var token = TokenHelper.GenerateToken(newUser.Email ?? string.Empty);
            _emailSender.SendEmail(registerDto.Email, EmailType.Registration, token);
            await _userCollection.InsertOneAsync(newUser);

            return newUser;
        }

        public async Task<ErrorOr<UserModel>> RegisterWithMobile(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Phone))
            {
                return Errors.User.InvalidPhone;
            }

            var phoneExists = await _userCollection.Find(u => u.Phone == registerDto.Phone).FirstOrDefaultAsync();
            if (phoneExists != null)
            {
                return Errors.User.PhoneAlreadyExists;
            }

            if (!string.IsNullOrWhiteSpace(registerDto.Email))
            {
                var emailExists = await _userCollection.Find(u => u.Email == registerDto.Email).FirstOrDefaultAsync();
                if (emailExists != null)
                {
                    return Errors.User.EmailAlreadyExists;
                }
            }

            var newUser = new UserModel
            {
                Email = registerDto.Email ?? null,
                Phone = registerDto.Phone,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Pnr = PasswordHelper.HashPassword(registerDto.Password),
                Roles = new List<string> { "User" },
                Country = registerDto.Country,
                Status = "Unverified",
                CreatedAt = DateTime.UtcNow
            };

            var token = TokenHelper.GenerateToken(newUser.Phone ?? string.Empty);
            await _userCollection.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<ErrorOr<bool>> ForgotPassword(string emailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(emailOrPhone))
            {
                return Error.Validation("InvalidEmailOrPhone", "Email or phone is required.");
            }

            var user = await _userCollection.Find(u => u.Email == emailOrPhone || u.Phone == emailOrPhone).FirstOrDefaultAsync();
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            var token = TokenHelper.GenerateToken(emailOrPhone);
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                _emailSender.SendEmail(user.Email, EmailType.PasswordReset, token);
            }

            return true;
        }

        public async Task<ErrorOr<bool>> ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                return Error.Validation("InvalidInput", "Token and new password are required.");
            }

            if (!TokenHelper.ValidateToken(token, out var emailOrPhone))
            {
                return Error.Validation("InvalidToken", "Token is invalid or expired.");
            }

            var user = await _userCollection.Find(u => u.Email == emailOrPhone || u.Phone == emailOrPhone).FirstOrDefaultAsync();
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            user.Pnr = PasswordHelper.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);

            return true;
        }

        public async Task<ErrorOr<bool>> VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Error.Validation("InvalidToken", "Token is required.");
            }

            if (!TokenHelper.ValidateToken(token, out var email))
            {
                return Error.Validation("InvalidToken", "Token is invalid or expired.");
            }

            var user = await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            if (user.Status == "Verified")
            {
                return Error.Validation("AlreadyVerified", "Email is already verified.");
            }

            user.Status = "Verified";
            user.UpdatedAt = DateTime.UtcNow;
            await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                _emailSender.SendEmail(user.Email, EmailType.Verification);
            }

            return true;
        }


        public Task<ErrorOr<bool>> VerifyMobile(string token)
        {
            // Logic to verify mobile token
            return Task.FromResult<ErrorOr<bool>>(true);
        }

        public async Task<ErrorOr<UserModel>> CreateOrUpdateProfile(ProfileDto profileDto)
        {
            var user = await _userCollection.Find(u => u.Id == profileDto.Id).FirstOrDefaultAsync();
            if (user == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            user.FirstName = profileDto.FirstName;
            user.LastName = profileDto.LastName;
            user.Country = profileDto.Country;
            user.UpdatedAt = DateTime.UtcNow;

            await _userCollection.ReplaceOneAsync(u => u.Id == profileDto.Id, user);
            return user;
        }

        public async Task<ErrorOr<UserModel>> UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Error.Validation("InvalidId", "The user ID cannot be null or empty.");
            }

            var existingUser = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return Error.NotFound("UserNotFound", "User not found.");
            }

            // Update only allowed fields
            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
                existingUser.Email = updateUserDto.Email;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Phone))
                existingUser.Phone = updateUserDto.Phone;

            if (!string.IsNullOrWhiteSpace(updateUserDto.FirstName))
                existingUser.FirstName = updateUserDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.LastName))
                existingUser.LastName = updateUserDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Country))
                existingUser.Country = updateUserDto.Country;

            if (updateUserDto.Addresses != null)
                existingUser.Addresses = updateUserDto.Addresses;

            existingUser.UpdatedAt = DateTime.UtcNow;
            // existingUser.UpdatedBy = "System"; // Esto puede ajustarse según el contexto

            var updateResult = await _userCollection.ReplaceOneAsync(
                filter: u => u.Id == id,
                replacement: existingUser
            );

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return Error.Failure("UpdateFailed", "Failed to update the user.");
            }

            return existingUser;
        }

        public async Task<ErrorOr<List<UserModel>>> GetAllUsers()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users;
        }

    }

}