using ErrorOr;
using Payments.Apps.User.Models;
using Payments.DTOs;

namespace Payments.Apps.User.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<UserModel>> LoginWithEmail(string email, string password);
        Task<ErrorOr<UserModel>> LoginWithMobile(string phone, string password);
        Task<ErrorOr<UserModel>> RegisterWithEmail(RegisterDto registerDto);
        Task<ErrorOr<UserModel>> RegisterWithMobile(RegisterDto registerDto);
        Task<ErrorOr<bool>> ForgotPassword(string emailOrPhone);
        Task<ErrorOr<bool>> ResetPassword(string token, string newPassword);
        Task<ErrorOr<bool>> VerifyEmail(string token);
        Task<ErrorOr<bool>> VerifyMobile(string token);
        Task<ErrorOr<UserModel>> CreateOrUpdateProfile(ProfileDto profileDto);
        Task<ErrorOr<UserModel>> UpdateUser(string id, UpdateUserDto updatedUserDto);
        Task<ErrorOr<List<UserModel>>> GetAllUsers();
    }
}
