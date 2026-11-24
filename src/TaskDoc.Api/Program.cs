using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskDoc.Api.Auth;
using TaskDoc.Api.Data;
using TaskDoc.Api.Endpoints;
using TaskDoc.Api.Users;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

// Data access (Dapper + SQLite)
DapperTypeHandlers.Register();
builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<RefreshTokenRepository>();

// Authentication: local users (DB password hash) and AD users (LDAP bind), both issued our own JWT.
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<SigningKeyProvider>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IAdAuthenticator, LdapAdAuthenticator>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<SigningKeyProvider, IOptions<AuthOptions>, TimeProvider>((jwtBearer, signingKey, auth, clock) =>
    {
        var clockSkew = TimeSpan.FromSeconds(30);
        jwtBearer.MapInboundClaims = false;
        jwtBearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = auth.Value.Jwt.Issuer,
            ValidAudience = auth.Value.Jwt.Audience,
            IssuerSigningKey = signingKey.Key,
            NameClaimType = "name",
            RoleClaimType = "role",
            ClockSkew = clockSkew,
            // Check expiry with the app's TimeProvider (the token library would otherwise use the system clock),
            // so token issue and validation always agree on the time.
            LifetimeValidator = (notBefore, expires, _, _) =>
            {
                var now = clock.GetUtcNow().UtcDateTime;
                return (notBefore is null || notBefore <= now + clockSkew)
                    && (expires is null || expires > now - clockSkew);
            },
        };
    });

// Every endpoint requires a signed-in user unless it opts out with AllowAnonymous().
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
    .AddPolicy(UserEndpoints.AdminPolicy, policy => policy.RequireRole(TokenService.AdminRole));

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// The WASM client is served from a different origin, so it needs CORS.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

DatabaseMigrator.Migrate(connectionString, app.Logger);
await InitialAdminSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.Run();

// Exposed for WebApplicationFactory in TaskDoc.Api.Tests.
public partial class Program;
