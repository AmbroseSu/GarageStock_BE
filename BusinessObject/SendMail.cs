using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObject.Enums;

namespace BusinessObject;

public class SendMail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid SendMailId { get; set; }
    // ===== Mail info =====
    [Required]
    public string ToEmail { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Body { get; set; } // nội dung ĐÃ render (snapshot)

    // ===== Status =====
    public SendMailStatus Status { get; set; } // Pending / Processing / Success / Failed

    // ===== Error handling =====
    public string? ErrorMessage { get; set; }

    // ===== Tracking =====
    public DateTime CreatedAt { get; set; }     // lúc tạo record (Pending)
    public DateTime? ProcessedAt { get; set; }  // lúc Hangfire bắt đầu xử lý
    public DateTime? SentAt { get; set; }       // lúc gửi thành công / thất bại

    // ===== Optional: trace Hangfire =====
    public string? HangfireJobId { get; set; }
    
    // ===== Template =====
    public Guid? MailTemplateId { get; set; }
    public MailTemplate? MailTemplate { get; set; }
}