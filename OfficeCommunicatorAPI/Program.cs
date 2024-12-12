using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeCommunicatorAPI.Services;


var builder = WebApplication.CreateBuilder(args);
string connectionString;
string jwtKey;
string passwordKey;

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException();
    jwtKey = builder.Configuration.GetSection("AuthSetting:TokenKey").Value ?? throw new ArgumentException();
    passwordKey = builder.Configuration.GetSection("AuthSetting:passwordKey").Value ?? throw new ArgumentException();
}
else if (builder.Environment.IsEnvironment("DockerEnv"))
{
    connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new ArgumentException();
    jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentException();
    passwordKey = Environment.GetEnvironmentVariable("PASSWORD_KEY") ?? throw new ArgumentException();
}
else if(builder.Environment.IsEnvironment("AzureEnv"))
{
    connectionString = Environment.GetEnvironmentVariable("AzureConnection") ?? throw new ArgumentException();
    jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentException();
    passwordKey = Environment.GetEnvironmentVariable("PASSWORD_KEY") ?? throw new ArgumentException();
}
else
{
    connectionString = builder.Configuration.GetConnectionString("ConnectionStrings:ProductionConnection") ?? throw new ArgumentException();
    jwtKey = builder.Configuration.GetSection("AuthSetting:TokenKey").Value ?? throw new ArgumentException();
    passwordKey = builder.Configuration.GetSection("AuthSetting:PasswordKey").Value ?? throw new ArgumentException();
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<OfficeDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddSingleton<DapperDbContext>(sp => new DapperDbContext(connectionString));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddSingleton<AuthHelper>(sp => new AuthHelper(jwtKey, passwordKey));


builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("MyPolicy", p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


builder.Services.AddSignalR();

WebApplication app = builder.Build();

// app.UseMiddleware<ExceptionMiddlewareHandler>();
//app.UseCors("DevCors");
app.UseCors("MyPolicy");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OfficeDbContext>();
    context.Database.EnsureCreated();
}

app.MapHub<CommunicatorHub>("/chatHub");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
