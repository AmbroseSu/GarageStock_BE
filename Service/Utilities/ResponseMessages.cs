namespace Service.Utilities;

public class ResponseMessages
{
    public const string CreatedSuccessfully = "Successfully created";
    public const string UpdateSuccessfully = "Successfully updated";
    public const string GetSuccessfully = "Successfully get";
    public const string DeleteSuccessfully = "Successfully deleted";
    public const string OperationFailed = "Operation failed";
    public const string ValueNull = "Please insert a valid value";
    public const string FailedToSaveData = "Unable to save data to database";
    public const string Unauthorized = "Unauthorized access";
    
    //User
    public const string EmailFormatInvalid = "Invalid email format";
    public const string EmailNotExists = "Email not exists";
    public const string EmailAlreadyExists = "Email already exists";
    public const string EmailOrUsernameNotExists = "Email or username not exists";
    public const string PasswordNotExists = "Password not exists";
    public const string UserHasDeleted = "User has been deleted";
    public const string UserNameAlreadyExists = "Username already exists";
    public const string PhoneNumberFormatInvalid = "Invalid phone number format";
    public const string EmailIsRequired = "Email is required";
    public const string UsernameIsRequired = "Username is required";
    public const string FullNameIsRequired = "Full name is required";
    public const string UserNotFound = "User not found";
    
    //Authentication
    public const string RefreshRequired = "Refresh token is required";
    public const string InvalidOrExpiredRefreshToken = "Invalid or expired refresh token";
    public const string IfEmailExists = "If email exists, OTP has been sent";
    public const string InvalidOtp = "Invalid OTP";
    public const string OtpExpired = "OTP has expired";
    public const string OtpBlocked = "Too many invalid attempts. OTP is blocked.";
    public const string OtpVerified = "OTP verified successfully";
    public const string ResetTokenInvalidOrExpired = "Invalid or expired reset token";
    public const string PasswordResetSuccessfully = "Password has been reset successfully";
    public const string ResetTokenIsRequired = "Reset token is required";
    public const string PasswordIsRequired = "Password is required";
    public const string ConfirmPasswordIsRequired = "Confirm password is required";
    public const string PasswordConfirmNotMatch = "Password and confirm password do not match";
    public const string PasswordTooWeak = "Password is too weak";
    public const string OldPasswordInvalid = "Old password is incorrect";
    public const string NewPasswordMustBeDifferent = "New password must be different from old password";
    public const string PasswordChangedSuccessfully = "Password changed successfully";
    public const string RefreshTokenIsRequired = "Refresh token is required";
    public const string LogoutSuccessfully = "Logged out successfully";
    
}