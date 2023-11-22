namespace Identity_Server.Constants;

public static class Account
{

    public const string UserNotFound = "User is not found";

    public static class LogInMessages
    {
        public const string LogInSuccess = "Successfully LogIn";
        public const string EmailVerificationRequired = "Your account is not verified. Please check your email for verify your account.";
        public const string RegisterAccountRequired = "There is no account register with this email. Please, Create an Account for LogIn.";
        public const string WrongCredentials = "Incorrect password";
    }

    public static class RegistrationMessages
    {
        public const string EmailExist = "An account already exist with this email. Please, Choose another account to register.";
        public const string UserCreatedSuccessfully = "User Created Successfully";
        public const string CheckEmailToVerifyAccount = "Please check your email to verify your account";
    }

    public static class EmailSendingMessages
    {
        public const string Failed = "Failed to send email";
    }

}
