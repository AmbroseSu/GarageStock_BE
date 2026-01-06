using BusinessObject;
using BusinessObject.Enums;

namespace Service;

public interface ISendMailService
{
    Task<Guid> EnqueueByTemplateAsync(
        string toEmail,
        MailTemplateCode templateCode,
        Dictionary<string, string> data,
        Guid? triggeredByUserId = null);

    Task<Guid> EnqueueSignupEmail(Users user, string plainPassword);
    Task<Guid> EnqueueForgotPasswordEmail(Users user, string otpCode);

    Task EnqueueJobAsync(Guid sendMailId);
}