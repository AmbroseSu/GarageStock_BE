using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using BusinessObject;
using DataAccess.DTO;
using DataAccess.DTO.Request;
using DataAccess.DTO.Response;
using Repository;
using Service.Utilities;

namespace Service.Impl;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ISendMailService _sendMailService;
    
    public UserService(IMapper mapper, IUnitOfWork unitOfWork, IUserRepository userRepository, ISendMailService sendMailService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _sendMailService = sendMailService;
    }
    
    public async Task<ResponseDto> CreateUser(UserRequest userRequest)
    {
        try
        {
            if (userRequest == null) throw new ArgumentNullException(nameof(userRequest));
            
            userRequest.Email = userRequest.Email?.Trim();
            userRequest.Username = userRequest.Username?.Trim();
            userRequest.PhoneNumber = userRequest.PhoneNumber?.Trim();
            
            if (string.IsNullOrWhiteSpace(userRequest.Email))
                return ResponseUtil.Error(ResponseMessages.EmailIsRequired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(userRequest.Username))
                return ResponseUtil.Error(ResponseMessages.UsernameIsRequired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(userRequest.FullName))
                return ResponseUtil.Error(ResponseMessages.FullNameIsRequired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            if (!IsValidEmail(userRequest.Email))
                return ResponseUtil.Error(ResponseMessages.EmailFormatInvalid, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            if (!string.IsNullOrWhiteSpace(userRequest.PhoneNumber) && !IsValidPhoneNumber(userRequest.PhoneNumber))
                return ResponseUtil.Error(ResponseMessages.PhoneNumberFormatInvalid, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            
            var userExistEmail = await _unitOfWork.UserUOW.FindUserByEmailAsync(userRequest.Email);
            if (userExistEmail != null)
                return ResponseUtil.Error(ResponseMessages.EmailAlreadyExists, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            var userExistUserName = await _unitOfWork.UserUOW.FindUserByUsernameAsync(userRequest.Username);
            if (userExistUserName != null)
                return ResponseUtil.Error(ResponseMessages.UserNameAlreadyExists, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            var plainPassword = GenerateRandomString(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);


            var user = _mapper.Map<Users>(userRequest);
            user.Password = hashedPassword;
            user.IsDeleted = false;
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            
            
            Guid sendMailId;

            // 5) Transaction: create user + create SendMail (Pending) must be atomic
            await using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.UserUOW.AddAsync(user);
                var affected = await _unitOfWork.SaveChangesAsync();
                if (affected <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseUtil.Error(ResponseMessages.FailedToSaveData, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
                }

                // 6) Enqueue signup email (tạo SendMail record + enqueue job)
                // QUAN TRỌNG: để enqueue an toàn, tốt nhất tách làm 2 bước:
                // - trong transaction: tạo SendMail (Pending)
                // - sau commit: BackgroundJob.Enqueue(...)
                // => vì bạn đang enqueue ngay trong service, mình khuyên sửa SendMailService theo hướng hỗ trợ 2 bước.
                //
                // TẠM THỜI: Nếu bạn vẫn enqueue ngay, hãy đảm bảo transaction commit trước khi job chạy.
                // Thực tế Hangfire thường chạy sau vài ms nhưng vẫn có race-condition.
                //
                // CÁCH CHUẨN: dùng method CreateSendMailPendingAsync + EnqueueJobAfterCommit.
                //sendMailId = await _sendMailService.EnqueueSignupEmail(user, plainPassword);
                
                sendMailId = await _sendMailService.EnqueueSignupEmail(user, plainPassword);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResponseUtil.Error(ex.Message, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
            }
            
            await _sendMailService.EnqueueJobAsync(sendMailId);

            // 7) Response
            var result = _mapper.Map<UserDto>(user);

            // Nếu bạn muốn trả kèm info mail queue:
            // result.... (hoặc return object chứa sendMailId)
            return ResponseUtil.GetObject(result, ResponseMessages.CreatedSuccessfully, HttpStatusCode.Created, 1);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(ex.Message, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
        }
    }
    
    
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
    }
    
    public static string GenerateRandomString(int length)
    {
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string specialCharacters = "!@#$%^&*";

        var allCharacters = lowerCase + upperCase + numbers + specialCharacters;

        var random = new Random();
        var randomString = new char[length];

        for (var i = 0; i < length; i++) randomString[i] = allCharacters[random.Next(allCharacters.Length)];

        return new string(randomString);
    }
    
    
}