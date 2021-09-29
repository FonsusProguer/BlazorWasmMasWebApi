using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebWasm.Dto;

namespace WebWasm.HttpRepository
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
        Task Logout();
    }
}
