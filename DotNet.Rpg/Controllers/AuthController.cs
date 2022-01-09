using DotNet.Rpg.Data.Interface;
using DotNet.Rpg.Dtos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNet.Rpg.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult > Register([FromBody] UserRegisterDto registerDto)
        {
            var response = await _authRepository.Register(
                new Models.User() { 
                UserName = registerDto.userName
                }, registerDto.password);

            if (response.success)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRegisterDto registerDto)
        {
            var response = await _authRepository.Login(registerDto.userName, registerDto.password);

            if (response.success)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
