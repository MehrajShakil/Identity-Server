using Identity_Server.DTOs;

namespace Identity_Server.Interfaces;

public interface IAccountService
{
    Task<UserRegistrationResponse> RegisterUserAsync(UserRegistrationRequest user);
    Task<UserLoginResponse> LoginAsync(UserLoginRequest user);
}

