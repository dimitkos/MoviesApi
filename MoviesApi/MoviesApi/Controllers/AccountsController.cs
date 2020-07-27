using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.DTOs;
using MoviesApi.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper autoMapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _mapper = autoMapper;
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(UserToken), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new IdentityUser { UserName = model.EmailAddress, Email = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }

        [HttpPost("refreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> RefreshToken([FromBody] UserInfo model)
        {
            var userInfo = new UserInfo
            {
                EmailAddress = HttpContext.User.Identity.Name
            };

            return await BuildToken(userInfo);
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] PaginationDto paginationDto)
        {
            var usersQueryable = _context.Users.AsQueryable();
            usersQueryable = usersQueryable.OrderBy(x => x.Email);

            await HttpContext.InsertPaginationParametersInResponse(usersQueryable, paginationDto.RecordsPerPage);

            var users = await usersQueryable.Paginate(paginationDto).ToListAsync();

            var usersDto = _mapper.Map<List<UserDto>>(users);

            return Ok(usersDto);
        }

        [HttpGet("roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            var roles = await _context.Roles
                .Select(x => x.Name)
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPost("assignRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AssignRole(EditRoleDto editRoleDto)
        {
            var user = await _userManager.FindByIdAsync(editRoleDto.UserId);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));

            //another way is to use 
            //await _userManager.AddToRoleAsync(user, editRoleDto.RoleName);

            return NoContent();
        }

        [HttpPost("removeRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(EditRoleDto editRoleDto)
        {
            var user = await _userManager.FindByIdAsync(editRoleDto.UserId);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));

            //another way is to use 
            //await _userManager.RemoveFromRoleAsync(user, editRoleDto.RoleName);

            return NoContent();
        }

        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userInfo.EmailAddress),
                new Claim(ClaimTypes.Email,userInfo.EmailAddress)
            };

            var identityUser = await _userManager.FindByEmailAsync(userInfo.EmailAddress);
            var claimsDb = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
