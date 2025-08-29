using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers;
using Handlers.User;
using Lbbak_api.Helpers;
using Lbbak_api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;
using static Handlers.Helpers.Cloudinary;

namespace Lbbak_api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(EnvironmentVariable.DBConnection()));

            //MongoDB
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(EnvironmentVariable.MongoConnection());
            });

            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase("MediaStorage"); // change name if needed
            });

            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
            });

            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Initiate>();
            });

            //AutoMapper
            services.AddAutoMapper(typeof(GetAllUsers.Handler).Assembly);

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. Use format: X-API-Key: {your_api_key}",
                    In = ParameterLocation.Header,
                    Name = "X-API-Key",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token **_only_**",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
                });
            });

            // JWT Authentication
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            // Custom Services
            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<UserDataLibrary, UserDataLibrary>();
            services.AddScoped<AdminDataLibrary, AdminDataLibrary>();
            services.AddScoped<CardDataLibrary, CardDataLibrary>();
            services.AddScoped<EventDataLibrary, EventDataLibrary>();
            services.AddScoped<OfferDataLibrary, OfferDataLibrary>();
            services.AddScoped<CityDataLibrary, CityDataLibrary>();
            services.AddSingleton<IMediaService, MediaService>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();

        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
