using AspNetCore_WebApi.Api.Models;
using AspNetCore_WebApi.Common.Exceptions;
using AspNetCore_WebApi.Common.Utilities;
using AspNetCore_WebApi.Data.Contracts;
using AspNetCore_WebApi.Entities;
using AspNetCore_WebApi.Services.Services;
using AspNetCore_WebApi.WebFramework.Api;
using AspNetCore_WebApi.WebFramework.Filters;
using Elmah;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore_WebApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly IJwtService jwtService;

        private readonly ILogger<UserController> logger;

        public UserController(IUserRepository userRepository, 
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,
            ILogger<UserController> logger, 
            IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.jwtService = jwtService;
        }

        [Authorize]
        public async Task<List<User>> Get(CancellationToken cancellationToken)
        {
            var userName = HttpContext.User.Identity.GetUserName();
            userName = HttpContext.User.Identity.Name;
            var userId = HttpContext.User.Identity.GetUserId();
            var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return users;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ApiResult<User>> Get(int id,CancellationToken cancellationToken)
        {
            var user2 = await userManager.FindByIdAsync(id.ToString());
            var role = await roleManager.FindByNameAsync("Admin"); 
            var user = await userRepository.GetByIdAsync(cancellationToken,id);
            if (user == null)
                return NotFound();

            await userManager.UpdateSecurityStampAsync(user);
            //await userRepository.UpdateSecuirtyStampAsync(user, cancellationToken);

            return user;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<string> Token(string username, string password,CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(username); 
            //var user = await userRepository.GetByUserAndPass(username, password, cancellationToken);
            if (user == null)
                throw new BadRequestException("نام کاربری یا پسورد اشتباه است.");
            var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا پسورد اشتباه است.");

            var jwt = await jwtService.GenerateAsync(user);
            return jwt;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Creat(UserDto userDto,CancellationToken cancellationToken)
        {
            //HttpContext.RiseError(new Exception("Your Exception"));
            //ErrorSignal.FromCurrentContext().Raise(new Exception("Your Exception"));
            logger.LogError("this is log test :)");
            //var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
            //if (exists)
            //    return BadRequest("user is exist.");
            var user = new User
            {
                Age = userDto.Age,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            var result = await userManager.CreateAsync(user, userDto.Password);

            var result2 = await roleManager.CreateAsync(new Role
            {
                Name = "Admin",
                Description = "admin role"
            });
            var result3 = await userManager.AddToRoleAsync(user, "Admin");
            await userRepository.AddAsync(user,userDto.Password, cancellationToken);
            return Ok(user);
        }

        [HttpPut]
        public async Task<ApiResult> Update(int id,User user,CancellationToken cancellationToken)
        {
            var updateUser = await userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user.UserName;
            updateUser.PasswordHash = user.PasswordHash;
            updateUser.FullName = user.FullName;
            updateUser.Age = user.Age;
            updateUser.Gender = user.Gender;
            updateUser.IsActive = user.IsActive;
            updateUser.LastLoginDate = user.LastLoginDate;

            await userRepository.UpdateAsync(updateUser, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            await userRepository.DeleteAsync(user, CancellationToken.None);
            return Ok();
        }
    }
}
