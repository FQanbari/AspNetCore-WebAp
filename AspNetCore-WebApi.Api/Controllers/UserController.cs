using AspNetCore_WebApi.Api.Models;
using AspNetCore_WebApi.Data.Contracts;
using AspNetCore_WebApi.Entities;
using AspNetCore_WebApi.WebFramework.Api;
using AspNetCore_WebApi.WebFramework.Filters;
using Elmah;
using ElmahCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ILogger<UserController> Logger { get; }

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            this.userRepository = userRepository;
            Logger = logger;
        }

        [HttpGet]        
        public async Task<List<User>> Get(CancellationToken cancellationToken)
        {
            var users = await userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return users;
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResult<User>> Get(int id,CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken,id);
            if (user == null)
                return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ApiResult<User>> Creat(UserDto userDto,CancellationToken cancellationToken)
        {
            //HttpContext.RiseError(new Exception("Your Exception"));
            //ErrorSignal.FromCurrentContext().Raise(new Exception("Your Exception"));
            Logger.LogError("this is log test :)");
            //var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
            //if (exists)
            //    return BadRequest("user is exist.");
            var user = new User
            {
                Age = userDto.Age,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName
            }; 
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
