using AspNetCore_WebApi.Entities;
using System.Threading.Tasks;

namespace AspNetCore_WebApi.Services.Services
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user);
    }
}