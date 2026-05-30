using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VirtualEventScheduler.API.DTOs;
using VirtualEventScheduler.API.Services;
using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API.Controllers
{
    /// <summary>
    /// Handles user authentication (register, login) and user management (list users, change roles).
    /// Passwords are hashed with BCrypt before storage.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user with the "Attendee" role.
        /// Returns a JWT token on success so the client can immediately authenticate.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(RegisterDto registerDto)
        {
            // Ensure email uniqueness using LINQ
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest(new { message = "Email already exists" });

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, 10),
                Role = "Attendee", // All new registrations start as Attendee
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate a JWT token so the user is immediately logged in
            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        /// <summary>
        /// Authenticates a user by email and password.
        /// Returns a JWT token used for all subsequent authorized requests.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Verify the bcrypt hash
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        /// <summary>
        /// Returns the full user list. Only accessible by Admin users.
        /// Uses LINQ to order users by ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Changes the role of a user (Admin, Staff, or Attendee).
        /// Admins cannot demote themselves to prevent lockout.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id:int}/role")]
        public async Task<ActionResult<UserDto>> UpdateUserRole(int id, UpdateRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Prevent an admin from removing their own admin role
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserIdClaim != null && int.Parse(currentUserIdClaim) == id && dto.Role != "Admin")
                return BadRequest(new { message = "You cannot demote yourself from Admin" });

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
