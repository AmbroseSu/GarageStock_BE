using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class MailTemplate
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid TemplateId { get; set; }

    [Required]
    public string TemplateCode { get; set; } // CREATE_USER, RESET_PASSWORD

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Body { get; set; } // HTML + {{Placeholder}}

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<SendMail>? SendMails { get; set; }
}