using DataAccess.DTO.Response;

namespace Service;

public interface IEmailService
{
    //Task<ResponseDto> SendEmail(string email, string subject, string content);
    Task SendAsync(string toEmail, string subject, string htmlContent);
}