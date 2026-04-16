using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TIKNA.Models;
using Microsoft.IdentityModel.Logging; // „Â„… ·≈ŸÂ«—  ›«’Ì· «·Œÿ√

var builder = WebApplication.CreateBuilder(args);

// ≈ŸÂ«—  ›«’Ì· «·Œÿ√ ›Ì «·‹ Console („Â„ Ãœ« ··„—Õ·… œÌ)
IdentityModelEventSource.ShowPII = true;

// 1. ≈÷«›… «·Œœ„« 
builder.Services.AddControllers();

// ≈⁄œ«œ «·‹ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TIKNA API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "÷⁄ «· Êﬂ‰ Â‰« „»«‘—… »œÊ‰ ﬂ·„… Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ≈⁄œ«œ ﬁ«⁄œ… «·»Ì«‰« 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ≈⁄œ«œ «·‹ Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2. ≈⁄œ«œ «·‹ JWT „⁄ ≈÷«›… «·‹ Debugging Events
var secretKey = "09686dfghgjkfmvnbbhu4768797784hvftyr8954hvbnncrfuirt"; // «·„› «Õ «·À«»  ·· Ã—»…
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        ClockSkew = TimeSpan.Zero
    };

    // --- Â–« «·Ã“¡ ”Ìﬂ‘› ·ﬂˆ «·Œÿ√ ›Ì ‘«‘… «·‹ Console ---
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("\n*****************************************");
            Console.WriteLine("›‘· «· ÊÀÌﬁ (Auth Failed)!");
            Console.WriteLine("«·”»» «· ﬁ‰Ì: " + context.Exception.Message);
            if (context.Exception.InnerException != null)
                Console.WriteLine(" ›«’Ì· ≈÷«›Ì…: " + context.Exception.InnerException.Message);
            Console.WriteLine("*****************************************\n");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("\n?  „ ﬁ»Ê· «· Êﬂ‰ »‰Ã«Õ ··„” Œœ„: " + context.Principal.Identity.Name);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"];
            Console.WriteLine("«· Êﬂ‰ «·„” ·„ „‰ Swagger: " + authHeader);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// 3.  — Ì» «·‹ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ⁄„· Seed ··‹ Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Student", "Company" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();