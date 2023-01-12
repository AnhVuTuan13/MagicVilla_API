using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private string secretKey;
        private IMapper mapper;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            this.roleManager = roleManager;
        }
        public bool IsUnqueUser(string username)
        {
            var user = db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if(user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid= await userManager.CheckPasswordAsync(user,loginRequestDTO.Password);
            if (user == null)
            {
               return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            var role = await userManager.GetRolesAsync(user);

            var tokenhandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, role.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenhandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenhandler.WriteToken(token),
                User = mapper.Map<UserDTO>(user),
                //Role = role.FirstOrDefault()
            };
            return loginResponseDTO;

        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Email= registerationRequestDTO.UserName,
                Name = registerationRequestDTO.Name,
                NormalizedEmail =registerationRequestDTO.UserName.ToUpper()
            };
            try
            {
                var result = await userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await roleManager.CreateAsync(new IdentityRole("admin"));
                        await roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await userManager.AddToRoleAsync(user, "admin");
                    var userToReturn= db.ApplicationUsers
                        .FirstOrDefault(u=>u.UserName== registerationRequestDTO.UserName);
                    return mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch(Exception ex)
            {

            }
           
          
            return new UserDTO();
        }
    }
}
