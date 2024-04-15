using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.EnumMapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using SPPA.Database;
using SPPA.Domain;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Repository.Orders;
using SPPA.Domain.Repository.Reports;
using SPPA.Domain.Repository.Users;
using SPPA.Domain.Repository.Workspaces;
using SPPA.Logic.Extensions;
using SPPA.Logic.Mapping;
using SPPA.Logic.Services;
using SPPA.Logic.UseCases.Workspaces;
using SPPA.Repository.Orders;
using SPPA.Repository.Reports;
using SPPA.Repository.Users;
using SPPA.Repository.Workspaces;
using SPPA.Web.Extensions;
using SPPA.Web.Filters;
using SPPA.Web.Identity;
using Swashbuckle.AspNetCore.Filters;
using System.IO.Compression;
using System.Text;

namespace SPPA.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var logger = NLog.LogManager.Setup()
                         .LoadConfigurationFromXml("nlog.config")
                         .GetCurrentClassLogger();
        logger.Info("Start main");

        try
        {
            // Load app configuration
            logger.Info("Load configuration");
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("licenselimit.json", true, true);

            builder.Logging.ClearProviders();
            builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

            builder.Services.Configure<AppSettings>(builder.Configuration);
            var appSettings = builder.Configuration.Get<AppSettings>();

            logger.Info("Configure services");
            ConfigureServices(builder, appSettings);
            var app = builder.Build();

            logger.Info("Configure Pipeline");
            ConfigurePipeline(app, appSettings);

            logger.Info("Post initialization");
            await PostInitAsync(app);

            logger.Info("Run web-application");
            await app.RunAsync();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Stopped program because of exception");
            throw;
        }
        finally
        {
            NLog.LogManager.Flush();
            NLog.LogManager.Shutdown();
        }
    }

    private static void ConfigureServices(WebApplicationBuilder builder, AppSettings appSettings)
    {
        // Add services to the container.
        var connectionString = appSettings.Database.Connection
                               ?? throw new InvalidOperationException("Connection string not found.");
        builder.Services.AddDbContextPool<ApplicationDbContext>(efOptions =>
        {
            efOptions.UseNpgsql(connectionString, npgOptions =>
            {
                //npgOptions.UseNodaTime();
            });
#if DEBUG
            efOptions.EnableSensitiveDataLogging(true);
#endif
        },
            poolSize: 20
        );

        builder.Services.AddCors();
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestBody |
                                    HttpLoggingFields.RequestPath |
                                    HttpLoggingFields.RequestMethod |
                                    HttpLoggingFields.ResponseStatusCode;
        });
        builder.Services.AddControllersWithViews(options =>
               {
                   options.Filters.Add<HttpResponseMfExceptionFilter>();
                   options.UseMfValidationMessage();
               })
               .AddNewtonsoftJson(opts =>
               {
                   opts.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                   //opts.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
               })
               .UseMfValidationResponseModel();

        builder.Services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);

        builder.Services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[] { "application/x-step", "application/json" };
            }
        );
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });
        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        // Auth
        builder.Services.AddAuthorization()
               //.AddIdentityCore<IdentityUser>();
               .AddIdentity<User, UserRole>(options =>
               {
                   options.Password.RequireDigit = false;
                   options.Password.RequireLowercase = false;
                   options.Password.RequireUppercase = false;
                   options.Password.RequireNonAlphanumeric = false;
                   options.Password.RequiredLength = 6;
                   options.User.RequireUniqueEmail = true;
                   options.Lockout.AllowedForNewUsers = false;
                   options.SignIn.RequireConfirmedEmail = true;
               })
               .AddUserStore<SPPAUserStore>()
               .AddRoleStore<SPPARoleStore>()
               .AddDefaultTokenProviders();

        var jwtSecretSid = appSettings.Authorization.JwtSecretSid
                           ?? throw new InvalidOperationException("Connection string not found.");
        builder.Services//.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = appSettings.ServerName,
                       RequireAudience = false,
                       ValidateAudience = false,
                       //ValidAudience = appSettings.ServerName,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretSid)),
                       ValidateIssuerSigningKey = true,
                       ValidateLifetime = true,
                   };
               });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SPPA API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>(false, JwtBearerDefaults.AuthenticationScheme);
            });
            builder.Services.AddSwaggerGenNewtonsoftSupport();
        }

        // Custom services
        builder.Services.AddAutoMapper(config =>
        {
            config.AddCollectionMappers();
            config.EnableEnumMappingValidation();
        },
        typeof(MappingProfile)
        );

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(WorkspaceCreateRequest).Assembly));
        builder.Services.AddSingleton<ILookupNormalizer, LowerLookupNormalizer>();

        builder.Services.AddScoped<UserAuthenticationService>()
               .AddTransient<EmailService>()
               .AddTransient<WorkspaceService>()
               .AddTransient<OrderService>()
               .AddTransient<InviteService>()
               .AddTransient<RoleService>()
               .AddSingleton<IReportStore, StimulsoftReportStore>()
               .AddScoped<LicensePlanLimitationsService>()
               .AddScoped<IWorkspaceRepository, WorkspaceRepository>()
               .AddScoped<IUserRepository, UserRepository>()
               .AddScoped<IUserSettingsRepository, UserSettingsRepository>()
               .AddScoped<IRoleRepository, RoleRepository>()
               .AddScoped<IOrderFileRepository, OrderFileRepository>()
               .AddScoped<IOrderRepository, OrderRepository>()
               .AddScoped<IInviteRepository, InviteRepository>();

    }

    private static void ConfigurePipeline(WebApplication app, AppSettings appSettings)
    {
        // Configure the HTTP request pipeline.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });


        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/api/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api/swagger";
                options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "SPPA API v1");
                options.EnablePersistAuthorization();
            });

            app.UseCors(corsPolicy =>
            {
                corsPolicy.AllowAnyOrigin();
                corsPolicy.AllowAnyMethod();
                corsPolicy.AllowAnyHeader();
            });
        }
        else
        {
            app.UseCors();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCompression();
        app.UseHttpLogging();
        app.MapControllers();

    }

    private static async Task PostInitAsync(WebApplication webApp)
    {
        using (var scope = webApp.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
                appSettings.Validate();

                // update DB
                var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                // create users and roles
                var authSettings = appSettings.Authorization;

                var admin = await db.Users.SingleOrDefaultAsync(x => x.Email.Equals(authSettings.AdminEmail));
                if (admin == null
                    && !string.IsNullOrWhiteSpace(authSettings.AdminEmail)
                    && !string.IsNullOrWhiteSpace(authSettings.AdminPassword)
                   )
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                    admin = new User(authSettings.AdminEmail)
                    {
                        EmailConfirmed = true
                    };
                    var res = await userManager.CreateAsync(admin, authSettings.AdminPassword);
                    if (!res.Succeeded)
                    {
                        throw new Exception("Fail on create user admin. Errors: " + res.Errors.ErrorsToString());
                    }
                }

                var role = await db.UserRoles
                                   .Where(x => x.UserId == admin.Id
                                               && x.RoleType == UserRoleTypeEnum.MainAdmin
                                               && x.WorkspaceId == null
                                               && x.OrderId == null
                                   )
                                   .SingleOrDefaultAsync();
                if (role == null)
                {
                    role = new UserRole(admin.Id, UserRoleTypeEnum.MainAdmin);
                    await db.UserRoles.AddAsync(role);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }

            // Check mapper configurations;
            var mapper = serviceProvider.GetRequiredService<IMapper>();
#if DEBUG
            //  mapper.ConfigurationProvider.AssertConfigurationIsValid();
#endif
            mapper.ConfigurationProvider.CompileMappings();
        }
    }

}
