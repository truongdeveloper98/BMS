using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.Models;
using WebAPI.Models.ViewModels;

namespace WebAPI.JwtFeatures
{
    public class JwtHandler
    {
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		protected readonly UsageDbContext _db;

		public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, UsageDbContext context)
		{
			_userManager = userManager;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_db = context;
		}

		private SigningCredentials GetSigningCredentials()
		{
			var key = Encoding.UTF8.GetBytes(_configuration["Config:TokenKey"]);
			var secret = new SymmetricSecurityKey(key);

			return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
		}

		private async Task<List<Claim>> GetClaims(ApplicationUser user)
		{
			var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
			var isPm = (from p in _db.Projects
						   join up in _db.UserProjects on p.ProjectId equals up.ProjectId
						   where up.UserId == user.Id && up.PositionId == Const.POSITION_PROJECT_PM
						select new ProjectByUserViewModel
						   {
							   ProjectId = p.ProjectId,
							   ProjectName = p.ProjectName
						   }).Distinct().ToList();

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Email),
				new Claim("Avatar", user.Avatar != null? url+'/'+user.Avatar.Replace("\\", "/"): ""),
				new Claim("DisplayName", user.DisplayName.ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim("UserID", user.Id.ToString()),
				new Claim("isPm", isPm.Count().ToString())
			};

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			return claims;
		}

		private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
		{
			var tokenOptions = new JwtSecurityToken(
				issuer: _configuration["Config:TokenIssuer"],
				audience: _configuration["Config:TokenAudience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Config:TokenExpired"])),
				signingCredentials: signingCredentials);

			return tokenOptions;
		}

		public async Task<string> GenerateToken(ApplicationUser user)
		{
			var signingCredentials = GetSigningCredentials();
			var claims = await GetClaims(user);
			var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
			var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

			return token;
		}

		public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings();
			var payload = new GoogleJsonWebSignature.Payload();
			try
			{
				settings.Audience = new List<string>() { 
						_configuration["GoogleAuthSettings:clientId"],
						_configuration["GoogleAuthSettings:clientIdiOS"],
						_configuration["GoogleAuthSettings:clientIdAndroid"],
						_configuration["GoogleAuthSettings:clientIdAndroid2"],
						_configuration["GoogleAuthSettings:clientIdAndroid3"]
				};

				payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
				return payload;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
