using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using CollabFlowApi;
using CollabFlowApi.Repositories;
using CollabFlowApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);

var databaseUrl = builder.Configuration["DATABASE_URL"] ?? throw new Exception("Data Source is missing from configuration");

var databaseUri = new Uri(databaseUrl);

string[] userInfo = databaseUri.UserInfo.Split(':');

var builders = new NpgsqlConnectionStringBuilder
{
    Host = databaseUri.Host,
    Port = databaseUri.Port,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = databaseUri.AbsolutePath.TrimStart('/'),
    SslMode = SslMode.Require,
};

var connectionString = builders.ToString();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICollaborationRepository, CollaborationRepository>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Auth Setup
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
        
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                var tokenType = claimsIdentity?.FindFirst("typ")?.Value;

                if (tokenType != "access")
                {
                    context.Fail("Invalid token type");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<ICollaborationRepository, CollaborationRepository>();

builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

// --- API Endpunkte ---

app.MapGet("/collaborations", async (ICollaborationRepository repo, ClaimsPrincipal user) =>
{
    var userId = GetUserFromToken(user);
    if (userId == null) return Results.Unauthorized();

    var items = await repo.GetAll(userId);
    return Results.Ok(items);
}).RequireAuthorization();

app.MapGet("/collaborations/{id}", async (string id, ICollaborationRepository repo, ClaimsPrincipal user) =>
{
    var userId = GetUserFromToken(user);

    if (userId == null) return Results.Unauthorized();
    var collab = await repo.GetById(id, userId);
    return collab is not null ? Results.Ok(collab) : Results.NotFound();
}).RequireAuthorization();


app.MapPut("/collaborations/", async (Collaboration collab, ICollaborationRepository repo, ClaimsPrincipal user) =>
{
    var userId = GetUserFromToken(user);

    if (userId == null) return Results.Unauthorized();
    
    await repo.AddOrUpdate(collab, userId);
    return Results.Ok(collab);
}).RequireAuthorization();

app.MapDelete("/collaborations/{id}", async (string id, ICollaborationRepository repo, ClaimsPrincipal user) =>
{
    var userId = GetUserFromToken(user);

    if (userId == null) return Results.Unauthorized();
    
    var deleted = await repo.Delete(id, userId);
    return deleted ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/collaborations", async (ICollaborationRepository repo, ClaimsPrincipal user) =>
{
    var userId = GetUserFromToken(user);

    if (userId == null) return Results.Unauthorized();
    await repo.Delete(userId);
    return Results.Ok();
}).RequireAuthorization();

app.Run();

static string? GetUserFromToken(ClaimsPrincipal claimsPrincipal)
{
    return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) 
           ?? claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);
}