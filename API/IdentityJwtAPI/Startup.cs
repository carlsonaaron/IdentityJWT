using JwtIdentityAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using JwtIdentityAPI.Services;
using Microsoft.OpenApi.Models;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using JwtIdentityAPI.Models.Account;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JwtIdentityAPI.Services.Authentication;
using System.Collections.Generic;
using System;

namespace JwtIdentityAPI
{
    public class Startup
    {
        readonly string allowSpecificOrigins = "_allowSpecificOrigins";
        

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));


            services
                .AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<IdentityContext>() 
                .AddDefaultTokenProviders(); // Required for storing tokens for Refresh Token 

            //services.AddOpenApiDocument(document =>
            //    document.PostProcess = d => d.Info.Title = "JWT/Identity API");


            // configure strongly typed JWT settings objects
            var jwtSettingsSection = Configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettingsSection);

            // configure jwt authentication
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRolesService, RolesService>();

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );


            services.AddCors(options =>
            {
                options.AddPolicy(allowSpecificOrigins,
                builder =>
                {
                    //builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    builder.WithOrigins("http://localhost:4200","https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT/Identity API", Version = "v1" });
                c.MapType<JObject>(() => new OpenApiSchema { Type = "object" });

                // define swagger docs and other options
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer authorization token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lowercase!!!
                    BearerFormat = "Bearer {token}",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    // defines scope - without a protocol use an empty array for global scope
                    { securityScheme, Array.Empty<string>() }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(allowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            /* =================== Add Swagger ==================== */
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "JwtIdentity API V1");
                //c.RoutePrefix = string.Empty;
            });
            /* =================== Add Swagger ==================== */


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
