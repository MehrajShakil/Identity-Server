using Identity_Server.Entities;

namespace Identity_Server.Services;

public interface IAccountService
{
    Task<bool> RegisterUserAsync(UserRequest user);
}

