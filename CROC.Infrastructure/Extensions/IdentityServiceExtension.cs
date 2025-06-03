using CROC.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace CROC.Infrastructure.Extensions;

public static class IdentityServiceExtension
{
	public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration config)
	{
		services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
		{
			options.Password.RequireDigit = false;
			options.Password.RequiredLength = 6;
			options.Password.RequireNonAlphanumeric = false;
		})
		.AddEntityFrameworkStores<AppDbContext>()
		.AddDefaultTokenProviders();

		var jwtSettings = config.GetSection("Jwt");
		var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(opt =>
		{
			opt.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidAudience = jwtSettings["Audience"],
				ValidIssuer = jwtSettings["Issuer"],
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};
		});

		return services;
	}
}