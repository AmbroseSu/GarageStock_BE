namespace BusinessObject.Enums;

public enum SendMailStatus
{
    Pending = 0,     // Chưa xử lý (mới insert, Hangfire chưa chạy)
    Processing = 1,  // Đã được Hangfire quét / đang xử lý
    Success = 2,     // Gửi thành công
    Failed = 3       // Gửi thất bại
}