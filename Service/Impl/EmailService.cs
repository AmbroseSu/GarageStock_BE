using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using DataAccess.DTO.Response;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Repository;
using Service.Utilities;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Service.Impl;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;
    
    public EmailService(IConfiguration config, IUnitOfWork unitOfWork)
    {
        _config = config;
        _unitOfWork = unitOfWork;
    }
    
    public async Task SendAsync(string toEmail, string subject, string htmlContent)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["EmailUserName"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlContent
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["EmailHost"],
            int.Parse(_config["EmailPort"] ?? "587"),
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailUserName"],
            _config["EmailPassword"]
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    private int GenerateOTP()
    {
        var random = new Random();
        var otp = random.Next(100000, 999999); // Tạo một số ngẫu nhiên từ 100000 đến 999999
        return otp;
    }
    
}