using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Authentication;
using WebAPI.Hubs;
using WebAPI.Mappers;
using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.Services;

namespace WebAPI {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddHttpContextAccessor();
            //SignalR
            builder.Services.AddSignalR(options => {
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.MaximumReceiveMessageSize = 20971520;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option => {
                option.SwaggerDoc("v1", new OpenApiInfo() {
                    Version = "v1",
                    Title = "PRM392",
                    Description = "This is a project of group PRN231 G3"
                });
                var securityScheme = new OpenApiSecurityScheme {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token: ",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                option.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                };

                option.AddSecurityRequirement(securityRequirement);
            });
            builder.Services
            .AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie( x => {
                x.ExpireTimeSpan = TimeSpan.FromSeconds(30);
                x.SlidingExpiration = true;
            })
            .AddGoogle(options => {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.SaveTokens = true;

                options.Scope.Add("profile");
                options.Scope.Add("email");
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
                x.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Ichat/User")) {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options => {
                options.AddPolicy("User", policy => policy.RequireRole("User"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            //Database
            builder.Services.AddDbContext<TDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Repository
            builder.Services.AddScoped<BlockedUserRepository>();
            builder.Services.AddScoped<ChatMemberRepository>();
            builder.Services.AddScoped<ChatRepository>();
            builder.Services.AddScoped<FileRepository>();
            builder.Services.AddScoped<FriendshipRepository>();
            builder.Services.AddScoped<MessageFlagRepository>();
            builder.Services.AddScoped<MessageRepository>();
            builder.Services.AddScoped<NotificationRepository>();
            builder.Services.AddScoped<ProfileRepository>();
            builder.Services.AddScoped<RoleRepository>();
            builder.Services.AddScoped<StatisticRepository>();
            builder.Services.AddScoped<StoryRepository>();
            builder.Services.AddScoped<UserAuthProviderRepository>();
            builder.Services.AddScoped<UserNotificationRepository>();
            builder.Services.AddScoped<UserRepository>();

            //Service
            builder.Services.AddScoped<BlockedUserService>();
            builder.Services.AddScoped<ChatMemberService>();
            builder.Services.AddScoped<ChatService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<FriendshipService>();
            builder.Services.AddScoped<MessageFlagService>();
            builder.Services.AddScoped<MessageService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<ProfileService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<StatisticService>();
            builder.Services.AddScoped<StoryService>();
            builder.Services.AddScoped<UserAuthProviderService>();
            builder.Services.AddScoped<UserNotificationService>();
            builder.Services.AddScoped<UserService>();

            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //Mapper
            builder.Services.AddAutoMapper(typeof(TMapperProfile).Assembly);

            //Jwt
            builder.Services.AddScoped<JwtHandler>();

            //CORS
            builder.Services.AddCors(options => {
                options.AddPolicy("Clients",
                    builder => {
                        builder.WithOrigins("https://localhost:7005")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            var app = builder.Build();

            app.UseCors("Clients");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<UserHub>("/Ichat/User");
            app.MapHub<AdminHub>("/Ichat/Admin");
            app.Run();
        }
    }
}