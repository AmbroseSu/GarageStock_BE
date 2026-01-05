using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using BusinessObject;
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
    
    public UserService(IMapper mapper, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }
    
    public async Task<ResponseDto> CreateUser(UserRequest userRequest)
    {
        try
        {
            if (userRequest == null) throw new ArgumentNullException(nameof(userRequest));

            var userExistEmail = await _unitOfWork.UserUOW.FindUserByEmailAsync(userRequest.Email);
            if (userExistEmail != null)
                return ResponseUtil.Error(ResponseMessages.EmailAlreadyExists, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            /*if (userExistEmail!.IsDeleted)
                return ResponseUtil.Error(ResponseMessages.UserHasDeleted, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);*/
            //var emailAttribute = new EmailAddressAttribute();
            /*if (IsValidEmail(userRequest.Email))
            {
                return ResponseUtil.Error(ResponseMessages.InvalidEmailFormat, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            }*/
            var userExistUserName = await _unitOfWork.UserUOW.FindUserByUsernameAsync(userRequest.Username);
            if (userExistUserName != null)
                return ResponseUtil.Error(ResponseMessages.UserNameAlreadyExists, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            /*if (userExistUserName!.IsDeleted)
                return ResponseUtil.Error(ResponseMessages.UserHasDeleted, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);*/

            if (!IsValidEmail(userRequest.Email))
                return ResponseUtil.Error(ResponseMessages.EmailFormatInvalid, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            if (!IsValidPhoneNumber(userRequest.PhoneNumber))
                return ResponseUtil.Error(ResponseMessages.PhoneNumberFormatInvalid, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            var plainPassword = GenerateRandomString(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);


            var user = _mapper.Map<Users>(userRequest);
            //user.Password = BCrypt.Net.BCrypt.HashPassword("Hieu1234");
            user.Password = hashedPassword;
            //user.PhoneNumber = userRequest.PhoneNumber;
            //user.DateOfBirth = userRequest.DateOfBirth;
            //user.FullName = userRequest.FullName;
            user.IsDeleted = false;
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            //await _unitOfWork.UserUOW.AddAsync(user);
            /*var saveChange = *///await _unitOfWork.SaveChangesAsync();
            /*if (saveChange > 0)
            {
                var userRole = new UserRole();
                userRole.UserId = user.UserId;
                userRole.RoleId = userRequest.RoleId;
                userRole.IsPrimary = true;
                await _unitOfWork.UserRoleUOW.AddAsync(userRole);
                var saveChange1 = await _unitOfWork.SaveChangesAsync();
                if (saveChange1 > 0)
                {
                    var emailContent = $"Xin chào {userRequest.UserName},<br><br>"
                                       + $"Tài khoản của bạn đã được tạo thành công.<br>"
                                       + $"Tên đăng nhập: <b>{userRequest.Email}</b><br>"
                                       + $"Mật khẩu: <b>{plainPassword}</b><br><br>"
                                       + $"Vui lòng đổi mật khẩu sau khi đăng nhập.";
                    var emailSent = false;
                    var retryCount = 0;
                    var maxRetries = 3;

                    //var emailResponse = await _emailService.SendEmail(userRequest.Email, "Tạo tài khoản thành công", emailContent);

                    while (!emailSent && retryCount < maxRetries)
                    {
                        var emailResponse = await _emailService.SendEmail(userRequest.Email, "Tạo tài khoản thành công",
                            emailContent);
                        if (emailResponse.StatusCode == (int)HttpStatusCode.Created)
                        {
                            emailSent = true;
                        }
                        else
                        {
                            retryCount++;
                            await Task.Delay(2000); // Chờ 2s trước khi thử lại
                        }
                    }

                    if (!emailSent)
                        return ResponseUtil.Error(ResponseMessages.FailedToSendEmail, ResponseMessages.OperationFailed,
                            HttpStatusCode.InternalServerError);

                    var result = _mapper.Map<UserDto>(user);
                    await _loggingService.WriteLogAsync(creatorId,
                        $"Tạo tài khoản thành công cho {userRequest.FullName} ({userRequest.Email})");
                    return ResponseUtil.GetObject(result, ResponseMessages.CreatedSuccessfully, HttpStatusCode.Created,
                        1);
                }

                return ResponseUtil.Error(ResponseMessages.FailedToSaveData, ResponseMessages.OperationFailed,
                    HttpStatusCode.InternalServerError);
            }*/

            return ResponseUtil.Error(ResponseMessages.FailedToSaveData, ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError);
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