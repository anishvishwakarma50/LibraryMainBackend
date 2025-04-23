using LBManage.Data;
using LBManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public AuthController(LibraryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Register API for Librarian and Student
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Role) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Role and Password are required.");
            }

            // ✅ Check if email already exists
            bool emailExists = await _context.Librarians.AnyAsync(l => l.Email == request.Email) ||
                               await _context.Students.AnyAsync(s => s.Email == request.Email);
            if (emailExists)
            {
                return BadRequest("User with this email already exists.");
            }

            string hashedPassword = _passwordHasher.HashPassword(null, request.Password);

            if (request.Role.ToLower() == "librarian")
            {
                var librarian = new Librarian
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Phone = request.Phone,
                    Address = request.Address,
                    HireDate = DateTime.Now
                };

                _context.Librarians.Add(librarian);
            }
            else if (request.Role.ToLower() == "student")
            {
                if (string.IsNullOrEmpty(request.EnrollmentNo) ||
                    string.IsNullOrEmpty(request.Department) ||
                    request.Year == null)
                {
                    return BadRequest("EnrollmentNo, Department, and Year are required for students.");
                }

                var student = new Student
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Phone = request.Phone,
                    Address = request.Address,
                    EnrollmentNo = request.EnrollmentNo,
                    Department = request.Department,
                    Year = request.Year,
                    CreatedAt = DateTime.Now
                };

                _context.Students.Add(student);
            }
            else
            {
                return BadRequest("Invalid role. Use 'librarian' or 'student'.");
            }

            await _context.SaveChangesAsync();
            return Ok("Registration successful.");
        }




        // ✅ Login API for Librarian and Student
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Role))
            {
                return BadRequest("Role is required.");
            }

            object user = null;
            string storedPasswordHash = "";
            string role = request.Role.ToLower();

            if (role == "librarian")
            {
                user = await _context.Librarians.FirstOrDefaultAsync(l => l.Email == request.Email);
            }
            else if (role == "student")
            {
                user = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
            }
            else
            {
                return BadRequest("Invalid role. Use 'librarian' or 'student'.");
            }

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            storedPasswordHash = (role == "librarian") ? ((Librarian)user).PasswordHash : ((Student)user).PasswordHash;
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null, storedPasswordHash, request.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid email or password.");
            }

            string token = GenerateJwtToken(request.Email, role);
            return Ok(new { Token = token });
        }

        // ✅ JWT Token Generation
        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ✅ Request Models
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } // librarian or student
        public string EnrollmentNo { get; set; } // Only for students
        public string Department { get; set; } // Only for students
        public int Year { get; set; } // Only for students
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // librarian or student
    }
}
