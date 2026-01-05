using AutoMapper;
using BusinessObject;
using BusinessObject.Enums;
using Hangfire;
using Repository;
using Service.Jobs;

namespace Service.Impl;

public class SendMailService : ISendMailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISendMailRepository _sendMailRepository;

    public SendMailService(IUnitOfWork unitOfWork, IMapper mapper, ISendMailRepository sendMailRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _sendMailRepository = sendMailRepository ?? throw new ArgumentNullException(nameof(sendMailRepository));
    }
    
    public async Task<Guid> EnqueueByTemplateAsync(
        string toEmail,
        MailTemplateCode templateCode,
        Dictionary<string, string> data,
        Guid? triggeredByUserId = null)
    {
        // 1. Lấy template theo code
        var template = await _unitOfWork.MailTemplateUOW
            .GetByCodeAsync(templateCode);

        if (template == null || !template.IsActive)
            throw new Exception($"MailTemplate '{templateCode}' not found or inactive");

        // 2. Render body
        var renderedBody = RenderTemplate(template.Body, data);

        // 3. Tạo SendMail (PENDING)
        var sendMail = new SendMail()
        {
            MailTemplateId = template.TemplateId,
            ToEmail = toEmail,
            Subject = template.Subject,
            Body = renderedBody,
            Status = SendMailStatus.Pending,
            CreatedAt = DateTime.UtcNow
            // TriggeredByUserId = triggeredByUserId (nếu bạn có field này)
        };

        await _unitOfWork.SendMailUOW.AddAsync(sendMail);
        await _unitOfWork.SaveChangesAsync();

        // 4. Enqueue Hangfire job
        /*BackgroundJob.Enqueue<SendMailJob>(
            job => job.Process(sendMail.SendMailId)
        );*/

        return sendMail.SendMailId;
    }
    
    public Task<Guid> EnqueueSignupEmail(Users user, string plainPassword)
    {
        return EnqueueByTemplateAsync(
            user.Email,
            MailTemplateCode.Sign_Up,
            new Dictionary<string, string>
            {
                ["FullName"] = user.FullName,
                ["Email"] = user.Email,
                ["Password"] = plainPassword
            }
        );
    }
    
    public void EnqueueJob(Guid sendMailId)
    {
        BackgroundJob.Enqueue<SendMailJob>(
            job => job.Process(sendMailId)
        );
    }
    
    private static string RenderTemplate(string template, Dictionary<string, string> data)
    {
        foreach (var item in data)
        {
            template = template.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return template;
    }
}