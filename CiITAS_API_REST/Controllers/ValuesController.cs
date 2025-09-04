using CiITAS_API_REST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;

namespace CiITAS_API_REST.Controllers
{
    [Route("api/citasApi/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ValuesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]

        public async Task<IActionResult> Login([FromBody] loginModel Login)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.EMAIL == Login.EMAIL && u.PASSWORD == Login.PASSWORD);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Correo o contraseña incorrecto" });


            }
            else
            {
                if (usuario.STATUS == "BL")
                {

                    return Unauthorized(new { message = "usuario bloqueado" });


                }

                if (usuario.STATUS == "EX")
                {

                    return Unauthorized(new { message = "Usuario expirado favor cambiar la contraseña" });


                }

                if (usuario.STATUS == "IN")
                {

                    return Unauthorized(new { message = "Usuario inactivo" });


                }

                else
                {
                    var key = "Only_PasswORD_For_Citas_Project_for_development_enviroments";
                    var keyBytes = Encoding.UTF8.GetBytes(key);

                    var claims = new[]
                    {
        new Claim("USER_ID", usuario.USER_ID.ToString()),
        new Claim("EMAIL", usuario.EMAIL),

    };


                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddMinutes(15),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new
                    {
                        message = "Login exitoso",
                        token = tokenString
                    });






                }
            }
        }
        //Empezar aqui 

        [HttpPost("createUser")]

        public async Task<IActionResult> CreateUser([FromBody] createUser model)
        {
            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    //Corregir con linq
                    //Hacer pruebas con pantalla
                    "EXEC createUser @P_name = {0}, @P_last_name = {1}, @P_second_last_name = {2}, @P_email = {3}, @P_password = {4}, @P_phone_number = {5}",
                    model.NAME,
                    model.LAST_NAME,
                    model.SECOND_LAST_NAME,
                    model.EMAIL,
                    model.PASSWORD,
                    model.PHONE_NUMBER
                );

                return Ok(new { message = "Usuario creado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
                   

            


