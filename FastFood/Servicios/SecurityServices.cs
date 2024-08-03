using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using FastFood.Rules;
using System.Configuration;
using Microsoft.AspNetCore.Hosting;
using FastFood.Models.User;


namespace Servicios;
public class SecurityServices : ISecurityServices
{

    private readonly ILogger<SecurityServices> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public SecurityServices( ILogger<SecurityServices> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _configuration = configuration;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    public User Login(string email, string password)
    {
        var rule = new UserRule(_configuration);
        var user = rule.GetUsuarioFromEmail(email);

        // Verificar si el usuario existe
        if (user == null)
        {
            // Log para cuando el usuario no existe
            _logger.LogWarning("El usuario con email {email} no existe.", email);
            return null;
        }

        // Retornar el usuario si las credenciales son válidas
        return user;
    }

}
