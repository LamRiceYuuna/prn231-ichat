using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Constants;
using WebAPI.Models;

namespace WebAPI.Authentication {
    public class JwtHandler {
        private IConfiguration configuration;
        public JwtHandler(IConfiguration configuration) {
            this.configuration = configuration;
        }
        public string GenerateToken(User user) {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var expire = DateTime.Now.AddDays(Convert.ToInt32(configuration["Jwt:ExpireInDays"]));
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = GenerateClaims(user),
                Issuer = issuer,
                Audience = audience,
                Expires = expire,
                SigningCredentials = credentials,
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(User user) {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(JwtType.UUID, user.UUID));
            claims.AddClaim(new Claim(JwtType.USERNAME, user.UserName ?? "none"));
            claims.AddClaim(new Claim(JwtType.EMAIL, user.Email ?? "none"));
            claims.AddClaim(new Claim(JwtType.IS_EMAIL_CONFIRMED, user.IsEmailConfirmed ? "true" : "false"));
            claims.AddClaim(new Claim(JwtType.HAS_PASSWORD, user.HasPassword ? "true" : "false"));
            claims.AddClaim(new Claim(JwtType.ROLE, user.Role.RoleName));
            claims.AddClaim(new Claim(JwtType.NICK_NAME, user.Profile.NickName));
            claims.AddClaim(new Claim(JwtType.AVATAR_URL, user.Profile.AvatarUrl));
            return claims;
        }
    }
}
