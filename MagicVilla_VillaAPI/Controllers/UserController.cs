using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepo;
        protected APIResponse response;

        public UsersController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
            response = new();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess= false;
                response.ErrorMessage.Add("username or pasword is incorrect");
                return BadRequest(response);
            }
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.IsSuccess= true;
            response.Result= loginResponse;
            return Ok(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUsernameUnquie = userRepo.IsUnqueUser(model.UserName);
            if(!ifUsernameUnquie)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess= false;
                response.ErrorMessage.Add("username already exists!");
                 return BadRequest(response);

            }
            var user = await userRepo.Register(model);
            if(user == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess= false;
                response.ErrorMessage.Add("Username alreadyn exists");
                return BadRequest(response);
            }
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.IsSuccess= true;
            return Ok(response);
        }
    }
}
