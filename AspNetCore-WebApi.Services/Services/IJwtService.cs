using AspNetCore_WebApi.Entities;

namespace AspNetCore_WebApi.Services.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}