using MedControl.Application.Services;
using MedControl.Domain.Entities;
using MedControl.Domain.Interfaces;
using MedControl.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedControl.API.Controllers
{
    [ApiController]
    [Route("api/med")]
    public class MedController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMedicoService _medicoService;
        private readonly IUsuariosMedicosService _usuariosMedicosService;

        public MedController(IConfiguration configuration, IMedicoService medicoService, IUsuariosMedicosService usuariosMedicosService)
        {
            _configuration = configuration;
            _medicoService = medicoService;
            _usuariosMedicosService = usuariosMedicosService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel request)
        {
            try
            {
                var user = await _medicoService.AuthenticateAsync(request.Crm, request.Password);
                if (user == null)
                    return Unauthorized();


                var token = GenerateJwtToken(user.Id.Value, user.Email);
                return Ok(new { token });

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RequestCreateMedicosModel register)
        {
            try
            {
                await _medicoService.CreateAsync(register);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RequestUpdateMedicoModel register)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;

                if (role != "doctor" && role != "admin")
                    return Forbid("Somente o perfil Médico poderá marcar consulta.");

                await _medicoService.UpdateAsync(register, userId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] MedicosModel register)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;

                if (role != "doctor" && role != "admin")
                    return Forbid("Somente o perfil Médico poderá marcar consulta.");

                await _medicoService.DeleteAsync(register.Id.Value);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getBySpecialty")]
        [Authorize]
        public async Task<IActionResult> GetBySpecialty([FromQuery] int specialtyId)
        {
            try
            {
                var medicos = await _medicoService.GetBySpecialty(specialtyId);
                return Ok(medicos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var medicos = await _medicoService.GetAllAsync();
                return Ok(medicos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string GenerateJwtToken(int medicoId, string email)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtConfig["Secret"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, medicoId.ToString()), //  ID do usuário
                new Claim(ClaimTypes.Role, "doctor"),
                new Claim(ClaimTypes.Name, email) //  E-mail do usuário
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtConfig["Issuer"],
                Audience = jwtConfig["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpGet("getAllSpecialties")]
        [Authorize]
        public async Task<IActionResult> GetAllSpecialty()
        {
            try
            {
                var medicos = await _medicoService.GetAllspecialtiesAsync();
                return Ok(medicos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}


