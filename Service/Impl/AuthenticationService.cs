using System.Net;
using AutoMapper;
using DataAccess.DTO;
using DataAccess.DTO.Request;
using DataAccess.DTO.Response;
using Repository;
using Service.Utilities;

namespace Service.Impl;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    
    public AuthenticationService(IJwtService jwtService, IEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResponseDto> SignIn(SignInRequest signInRequest)
    {
        try
        {
            var user = await _unitOfWork.UserUOW.FindUserByEmailAsync(signInRequest.EmailOrUsername) 
                       ?? await _unitOfWork.UserUOW.FindUserByUsernameAsync(signInRequest.EmailOrUsername);
            if (user == null || !BCrypt.Net.BCrypt.Verify(signInRequest.Password, user.Password))
                return ResponseUtil.Error(ResponseMessages.EmailOrUsernameNotExists + " or " + ResponseMessages.PasswordNotExists,
                    ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            if (user.IsDeleted)
                return ResponseUtil.Error(ResponseMessages.UserHasDeleted, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            if (!signInRequest.FcmToken.Equals(user.FcmToken) && !signInRequest.FcmToken.Equals("string"))
            {
                user.FcmToken = signInRequest.FcmToken;
            }

            var jwtToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user, new Dictionary<string, object>());
            
            var jwtAuthResponse = new JwtAuthenticationResponse();
            var userDto = _mapper.Map<UserDto>(user);
            
            jwtAuthResponse.UserDto = userDto;
            jwtAuthResponse.Token = jwtToken;
            jwtAuthResponse.RefreshToken = refreshToken;
            await _unitOfWork.UserUOW.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return ResponseUtil.GetObject(jwtAuthResponse, ResponseMessages.CreatedSuccessfully, HttpStatusCode.Created,
                0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}