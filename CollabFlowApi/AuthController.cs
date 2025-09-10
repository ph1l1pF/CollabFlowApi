using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using CollabFlowApi.Services;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly UserRepository _userRepository;

    public AuthController(TokenService tokenService, UserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    [HttpPost("apple")]
    public async Task<IActionResult> SignInWithApple([FromBody] AppleLoginRequest request)
    {
        var keys = await AppleKeysProvider.GetKeysAsync();
        var jwtHandler = new JwtSecurityTokenHandler();

        var token = request.IdentityToken;

        var jwt = jwtHandler.ReadJwtToken(token);

        var appleKey = keys.FirstOrDefault(k => k.kid == jwt.Header.Kid);
        if (appleKey == null) return Unauthorized("Key not found");

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(new RSAParameters
        {
            Modulus = Base64UrlEncoder.DecodeBytes(appleKey.n),
            Exponent = Base64UrlEncoder.DecodeBytes(appleKey.e)
        });

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://appleid.apple.com",
            ValidateAudience = false, 
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateLifetime = true
        };

        try
        {
            var principal = jwtHandler.ValidateToken(token, validationParams, out _);
            var sub = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
            if (sub == null) return Unauthorized("Invalid token");

            var user = await _userRepository.FindOrCreateAsync(sub);

            var tokenResponse = _tokenService.CreateTokens(user.Id);
            return Ok(new { tokenResponse });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        var user = _tokenService.ValidateToken(request.RefreshToken);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) 
               ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        var tokenResponse = _tokenService.CreateTokens(userId);

        return Ok(new
        {
            tokenResponse
        });
    }
}

public class AppleLoginRequest
{
    public string IdentityToken { get; set; }
}
