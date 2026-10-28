using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GuetoBarbearia.Api.Data;
using System.Text;
using GuetoBarbearia.Api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Login = GuetoBarbearia.Api.models.Login;

namespace GuetoBarbearia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly GuetoDbContext context;
        private readonly IConfiguration configuration;
        public LoginController(GuetoDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        

        [HttpPost("registro")]
        public async Task<ActionResult> Registro([FromBody] Login login)
        {
            
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Senha))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            if (login.Senha.Length < 6)
            {
                return BadRequest("Senha Muito Curta");
            }
            var usersExist = await context.Usuarios.AnyAsync(x => x.NomeUsuario == login.Usuario);

            if (usersExist)
            {
                return Conflict("Ja Existe um Usuario Registrado com Esses Dados");
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(login.Senha);

            var NovoUsuario = new Usuario()
            {
                NomeUsuario = login.Usuario,
                SenhaHash = senhaHash
            };
            context.Usuarios.Add(NovoUsuario);
            await context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensagem = "Usuario Registrado Com Sucesso!",
                usuario = NovoUsuario.NomeUsuario
            });
            
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            var usuarioBanco = await context.Usuarios.FirstOrDefaultAsync(x => x.NomeUsuario == login.Usuario);

            if (usuarioBanco == null || !BCrypt.Net.BCrypt.Verify(login.Senha, usuarioBanco.SenhaHash))
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }
            
            var jwtconfig = configuration.GetSection("Jwt");
            var jwtKey = jwtconfig["Key"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("usuario", login.Usuario),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtconfig["Issuer"],
                Audience = jwtconfig["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            var TokenString = tokenhandler.WriteToken(token);
            return Ok(new { token = TokenString });
        }
    }
}
