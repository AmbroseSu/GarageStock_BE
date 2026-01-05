using BusinessObject.Enums;
using Repository;

namespace Service.Jobs;

public class SendMailJob
{
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public SendMailJob(IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public async Task Process(Guid sendMailId)
    {
        var mail = await _unitOfWork.SendMailUOW.GetByIdAsync(sendMailId);
        if (mail == null) return;

        mail.Status = SendMailStatus.Processing;
        mail.ProcessedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        try
        {
            await _emailService.SendAsync(
                mail.ToEmail,
                mail.Subject,
                mail.Body
            );

            mail.Status = SendMailStatus.Success;
            mail.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            mail.Status = SendMailStatus.Failed;
            mail.ErrorMessage = ex.Message;
            mail.SentAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            throw; // ⚠️ để Hangfire retry
        }

        await _unitOfWork.SaveChangesAsync();
    }
}