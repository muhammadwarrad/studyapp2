using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using LearningStarter.Data;
using LearningStarter.Entities;
using LearningStarter.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LearningStarter;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers();

        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.MaxValue;
            options.Preload = true;
            options.IncludeSubDomains = true;
        });

        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

        });

        services.AddIdentity<User, Role>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                    options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                    options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
                })
            .AddEntityFrameworkStores<DataContext>();

        services.AddMvc();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Learning Starter Server",
                Version = "v1",
                Description = "Description for the API goes here.",
            });

            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.MapType(typeof(IFormFile), () => new OpenApiSchema { Type = "file", Format = "binary" });
        });

        services.AddSpaStaticFiles(config =>
        {
            config.RootPath = "learning-starter-web/build";
        });

        services.AddHttpContextAccessor();

        // configure DI for application services
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
    {
        dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();
        
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = true;
        });

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Learning Starter Server API V1");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(x => x.MapControllers());

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "learning-starter-web";
            if (env.IsDevelopment())
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:3001");
            }
        });
        
        using var scope = app.ApplicationServices.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>();

        SeedRoles(dataContext, roleManager).Wait();
        SeedUsers(dataContext, userManager).Wait();
        SeedFlashcardSets(dataContext);
        SeedUserFlashcardSet(dataContext);
        //SeedSetCards(dataContext);
        // SeedFlashcards(dataContext);
    }

    // public static void SeedFlashcards(DataContext dataContext)
    // {
    //     if (dataContext.Set<Flashcard>().Any())
    //     {
    //         return;
    //     }
    //
    //     var flashcardsToSeed = new List<Flashcard>()
    //     {
    //         new()
    //         {
    //             Term = "Term 1",
    //             Definition = "Definition 1",
    //         },
    //         new()
    //         {
    //             Term = "Term 2",
    //             Definition = "Definition 2",
    //         }
    //     };
    //     dataContext.Set<Flashcard>().AddRange(flashcardsToSeed);
    //     dataContext.SaveChanges();
    // }

    // public static void SeedSetCards(DataContext dataContext)
    // {
    //     if (dataContext.Set<FlashcardDto>().Any())
    //     {
    //         return;
    //     }
    //
    //     var setCardsToSeed = new List<FlashcardDto>()
    //     {
    //         new()
    //         {
    //             FlashcardSet = dataContext.Set<FlashcardSet>().First(),
    //             Flashcard = dataContext.Set<Flashcard>().First()
    //         }
    //     };
    //     dataContext.Set<CardSetCards>().AddRange(setCardsToSeed);
    //     dataContext.SaveChanges();
    // }
    private static void SeedUserFlashcardSet(DataContext dataContext)
    {
        if (dataContext.Set<UserFlashcardSet>().Any())
        {
            return;
        }

        var userFlashcardSetToSeed = new List<UserFlashcardSet>()
        {
            new()
            {
                User = dataContext.Set<User>().First(),
                FlashcardSet = dataContext.Set<FlashcardSet>().First()

            }
        };
        
        dataContext.Set<UserFlashcardSet>().AddRange(userFlashcardSetToSeed);
        dataContext.SaveChanges();
    }
    
    
    public static void SeedFlashcardSets(DataContext dataContext)
    {
        if (dataContext.Set<FlashcardSet>().Any())
        {
            return;
        }

        var flashcardSetToSeed = new List<FlashcardSet>()
        {
            new() {    
                
                Title = "Flashcard Set 1",
                Description = "Flashcard Set 1 Description",
                Flashcards = new List<FlashcardDto>().Select(x => new FlashcardDto
                {
                    Id = x.Id,
                    Front = x.Front,
                    Back = x.Back,
                }).ToList(),
                // Flashcards = new List<CardSetCards>
                // {
                //     new()
                //     {
                //         Flashcard = new Flashcard
                //         {
                //             Term = "Term 1",
                //             Definition = "Definition 1",
                //         }
                //     }
                // },
                
            },
            new()
            {
                Title = "Flashcard Set 2",
                Description = "Flashcard Set 2 Description",
                Flashcards = new List<FlashcardDto>
                {
                    
                },
            },
        };
        dataContext.Set<FlashcardSet>().AddRange(flashcardSetToSeed);
        dataContext.SaveChanges();
    }
    

    private static async Task SeedUsers(DataContext dataContext, UserManager<User> userManager)
    {
        var numUsers = dataContext.Users.Count();

        if (numUsers == 0)
        {
            var seededUser = new User
            {
                FirstName = "Seeded",
                LastName = "User",
                UserName = "admin",
                Email = "<EMAIL>",
                
            };

            await userManager.CreateAsync(seededUser, "Password");
            await userManager.AddToRoleAsync(seededUser, "Admin");
            await dataContext.SaveChangesAsync();
        }

        var userToSeed = new List<User>()
        {
            new()
            {
                FirstName = "Randy",
                LastName = "Orton",
                UserName = "randyOrton",
                Email = "randyOrton@gmail.com",
            }
        };
        dataContext.Set<User>().AddRange(userToSeed);
        dataContext.SaveChanges();
    }
    
    private static async Task SeedRoles(DataContext dataContext, RoleManager<Role> roleManager)
    {
        var numRoles = dataContext.Roles.Count();

        if (numRoles == 0)
        {
            var seededRole = new Role
            {
                Name = "Admin"
            };

            await roleManager.CreateAsync(seededRole);
            await dataContext.SaveChangesAsync();
        }
    }
}
