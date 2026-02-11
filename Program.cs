using Microsoft.AspNetCore.OpenApi;
using EmployeeHierarchy.Api.Data;
using EmployeeHierarchy.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // This works now with Native OpenAPI
using Scalar.AspNetCore;        // The new UI
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PositionService>();
// dep't
builder.Services.AddScoped<DashboardService>();

// dep't 
builder.Services.AddScoped<DepartmentService>();

// latter added to get all users
builder.Services.AddScoped<UserService>();
// 3. Authentication
var jwtKey = builder.Configuration["JwtSettings:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey!);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddControllers();

// 4. Modern OpenAPI (Replacing Swashbuckle)
builder.Services.AddOpenApi("v1", options => 
{
    // Configure JWT Security for the documentation
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Employee Hierarchy API",
            Version = "v1",
            Description = "Built with .NET 10 Native OpenAPI"
        };

        // Define Security Scheme
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter your JWT Token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", securityScheme);

        // Apply Security Globally
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
                Array.Empty<string>()
            }
        };
        document.SecurityRequirements.Add(securityRequirement);

        return Task.CompletedTask;
    });
});

// 5. CORS (Allow Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// --- PIPELINE ---

if (app.Environment.IsDevelopment())
{
    // Generate the OpenAPI JSON
    app.MapOpenApi();
    // Use the Scalar UI (Modern replacement for Swagger UI)
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("Employee API Docs");
    });
}

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();