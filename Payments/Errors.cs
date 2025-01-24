using ErrorOr;

namespace Payments
{
    public static class Errors
{
    public static class User
    {
        public static Error InvalidEmail => Error.Validation(
            code: "User.InvalidEmail",
            description: "The email provided is not valid."
        );

        public static Error InvalidPhone => Error.Validation(
            code: "User.InvalidPhone",
            description: "The phone provided is not valid."
        );

        public static Error EmailAlreadyExists => Error.Conflict(
            code: "User.EmailAlreadyExists",
            description: "A user with this email already exists."
        );

        public static Error PhoneAlreadyExists => Error.Conflict(
            code: "User.PhoneAlreadyExists",
            description: "A user with this phone already exists."
        );
    }
}

}
