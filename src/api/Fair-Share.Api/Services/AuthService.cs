using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Fair_Share.Api.Data;
using Fair_Share.Api.DTOs.Auth;
using Fair_Share.Api.Entities;
using Fair_Share.Api.Mappers;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Fair_Share.Api.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly AccountMapper _accountMapper;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            AccountMapper accountMapper
        )
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _accountMapper = accountMapper;
        }

        public async Task<AuthResponseDto?> AuthenticateWithGoogleAsync(string idToken)
        {
            try
            {
                _logger.LogInformation(
                    $"Client ID from config {_configuration["Authentication:Google:AndroidClientId"]}"
                );
                _logger.LogInformation("Token is " + idToken);
                // Verify Google ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience =
                        [
                            _configuration["Authentication:Google:AndroidClientId"]
                                ?? throw new InvalidOperationException(
                                    "Google AndroidClientId not configured"
                                )
                        ]
                    }
                );

                // Check if user exists
                var account = await _context
                    .Accounts.Include(a => a.Team)
                    .FirstOrDefaultAsync(a =>
                        a.GoogleId == payload.Subject || a.Email == payload.Email
                    );

                if (account == null)
                {
                    // Create new user
                    account = new Account
                    {
                        Name = payload.Name,
                        Email = payload.Email,
                        GoogleId = payload.Subject
                    };

                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "New user created via Google OAuth: {Email}",
                        account.Email
                    );
                }
                else if (account.GoogleId == null)
                {
                    // Link existing account to Google
                    account.GoogleId = payload.Subject;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Linked existing account to Google: {Email}",
                        account.Email
                    );
                }

                var isNewUser = checkIfNotInTeam(account);

                // Generate JWT
                var jwtKey =
                    _configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key not configured");
                var jwtIssuer =
                    _configuration["Jwt:Issuer"]
                    ?? throw new InvalidOperationException("JWT Issuer not configured");
                var jwtAudience =
                    _configuration["Jwt:Audience"]
                    ?? throw new InvalidOperationException("JWT Audience not configured");
                var token = JwtGenerator.GenerateJwtToken(account, jwtKey, jwtIssuer, jwtAudience);
                var response = _accountMapper.ToAuthResponseDto(account, token, isNewUser);

                return response;
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning("Invalid Google ID token: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                throw;
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Find user by email or name
            var account = await _context
                .Accounts.Include(a => a.Team)
                .FirstOrDefaultAsync(a => a.Email == request.Email);

            if (account == null || account.PasswordHash == null)
            {
                _logger.LogWarning("Login failed: User not found - {Email}", request.Email);
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password - {Email}", request.Email);
                return null;
            }

            var isNewUser = checkIfNotInTeam(account);

            // Generate JWT
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not configured");
            var token = JwtGenerator.GenerateJwtToken(account, jwtKey, jwtIssuer, jwtAudience);

            _logger.LogInformation("User logged in successfully: {Email}", account.Email);

            var response = _accountMapper.ToAuthResponseDto(account, token, isNewUser);

            return response;
        }

        public async Task<AuthResponseDto?> SignupAsync(SignupRequestDto request)
        {
            // Check if user already exists
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a =>
                a.Email == request.Email
            );

            if (existingAccount != null)
            {
                _logger.LogWarning("Signup failed: Email already exists - {Email}", request.Email);
                return null;
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new user
            var account = _accountMapper.ToEntity(request, passwordHash);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Generate JWT
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not configured");
            var token = JwtGenerator.GenerateJwtToken(account, jwtKey, jwtIssuer, jwtAudience);

            _logger.LogInformation("New user signed up: {Email}", account.Email);

            return new AuthResponseDto
            {
                Token = token,
                Email = account.Email,
                Name = account.Name,
                IsNewUser = true
            };
        }

        private bool checkIfNotInTeam(Account account)
        {
            return account.TeamId == null;
        }
    }
}
