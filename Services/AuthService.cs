using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly TaskManagerDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(TaskManagerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<string>> RegisterAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Verificar si el usuario ya existe
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == createUserDto.Username || u.Email == createUserDto.Email);

                if (existingUser != null)
                {
                    return ApiResponse<string>.ErrorResponse("El usuario o email ya existe");
                }

                // Crear nuevo usuario
                var user = new User
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);
                return ApiResponse<string>.SuccessResponse(token, "Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error al registrar usuario: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return ApiResponse<string>.ErrorResponse("Credenciales inválidas");
                }

                var token = GenerateJwtToken(user);
                return ApiResponse<string>.SuccessResponse(token, "Login exitoso");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error al iniciar sesión: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Usuario no encontrado");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error al obtener usuario: {ex.Message}");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("email", user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["ExpiryInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}