using AutoMapper;
using Azure.Storage.Blobs;
using Domain;
using Domain.Entities;
using EngineAPI.Behaviors;
using EngineAPI.Filters;
using EngineAPI.Services;
using EngineAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace EngineAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();//para evitar que el email token claim se solape con el de identity por defecto
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton(provider =>
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                }).CreateMapper());

            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)); //Este es el valor usado para hacer mediciones en el planeta tierra, cuando se hagan sistemas para marte deben tener esto en cuenta

            services.AddTransient<IStorageManager, AzureStorageSaver>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDataContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"),
                             sqlServer => sqlServer.UseNetTopologySuite()));

            services.AddIdentity<ApplicationUser, IdentityRole>(cfg =>
            {
                //  cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = false;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDataContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, //true, it need to be true
                    ValidateAudience = false, //true, it need to be true
                    ValidateLifetime = true, //this need to be deleted
                    ValidateIssuerSigningKey = true,
                    //ValidAudience = Configuration["AuthSettings:Audience"], // those 3 need to be uncomment
                    //ValidIssuer = Configuration["AuthSettings:Issuer"],
                    //RequireExpirationTime = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["keyjwt"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddAuthorization(options =>     //Para configurar que solo alguien con tal rol pueda acceder a un endpoint
            {
                options.AddPolicy("IsAdmin", //nombre de la politica que va a ser usada
                  policy => policy.RequireClaim("role", "admin,superuser"));  //el primer valor es el claim requerido y el segundo es un arreglo de valores permitidos para esa politica
            });
            // services.AddResponseCaching();
            //trasient es el tiempo mas corto de vida que le damos a un servicio y significa que cada vez que pidamos una instancia del servicio vamos a obtener una nueva instancia de ese reporsitorio en memoria, siempre sera una instancia nueva
            //add scopue, el tiempod e vida de la clase va a ser va a ser durante toda la peticion, si distintas clases piden el mismo serviico y lo hacen dentro del mismo contexto http, se les sirve la misma instancia
            //singleton el tiempo de vida de la instancia del servicio va a ser durante todo el tiempo de la ejecucion del proyecto, esto quiere decir que varios clientes pueden compartir la misma instancia de la clase instanciada 
            //services.AddTransient<IRepository, InMemoryRepository>();
            //services.AddScoped<WeatherForecastController>();
            //services.AddTransient<MyAccionFilter>();
            services.AddControllers(
                options =>
                {
                    options.Filters.Add(typeof(ExceptionFilter));
                    options.Filters.Add(typeof(BadRequestParser));//Para que cada badrequest sea interceptado y en lugar de devolver el badrequest simple, devuelva un listado de string de los errores que sea mas facil de manipular en el que va a consumir esa app
                }
                ).ConfigureApiBehaviorOptions(BehaviorBadRequests.Parse); //Esto es para que se sobreescriba el badrequest que viene desde los helpers internos del framework como ApiController
          
            services.AddRazorPages();

            services.AddScoped(_ => {
                return new BlobServiceClient(Configuration.GetConnectionString("AzureStorage"));
            });

            services.AddCors(options =>
            {
                var frontendUrl = Configuration.GetValue<string>("frontend_url");
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders(new string[] { "totalRecordsQuantity" });
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EngineAPI", Version = "v1" });
            });

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EngineAPI v1"));

            app.UseHttpsRedirection();

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("fr-FR"),
                new CultureInfo("es-ES"),
            };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"), //English US will be the default culture (for new visitors)
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseRouting();

            app.UseCors();
            // global cors policy
            //app.UseCors(x => x
            //   .SetIsOriginAllowed(origin => true)
            //   .AllowAnyMethod()
            //   .AllowAnyHeader()
            //   .AllowCredentials());
            //   app.UseResponseCaching();

            // global error handler
            //  app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            //  app.UseMiddleware<JwtMiddleware>();

            app.UseAuthentication(); //antes de autorizarte debo autenticarte

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //   endpoints.MapControllerRoute("default", "{culture:culture}/{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
