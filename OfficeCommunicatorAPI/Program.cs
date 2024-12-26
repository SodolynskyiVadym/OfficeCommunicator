using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeCommunicatorAPI.Services;


var builder = WebApplication.CreateBuilder(args);
string connectionString;
string jwtKey;
string passwordKey;
string azuriteBlobStorageConnectionString;

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException();
    azuriteBlobStorageConnectionString = builder.Configuration.GetSection("Azurite:BlobStorage").Value ?? throw new ArgumentException();
    jwtKey = builder.Configuration.GetSection("AuthSetting:TokenKey").Value ?? throw new ArgumentException();
    passwordKey = builder.Configuration.GetSection("AuthSetting:passwordKey").Value ?? throw new ArgumentException();
}
else if (builder.Environment.IsEnvironment("DockerEnv"))
{
    connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new ArgumentException();
    azuriteBlobStorageConnectionString = Environment.GetEnvironmentVariable("AZURITE_BLOB_STORAGE") ?? throw new ArgumentException();
    jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentException();
    passwordKey = Environment.GetEnvironmentVariable("PASSWORD_KEY") ?? throw new ArgumentException();
}
else if(builder.Environment.IsEnvironment("AzureEnv"))
{
    connectionString = Environment.GetEnvironmentVariable("AzureConnection") ?? throw new ArgumentException();
    azuriteBlobStorageConnectionString = Environment.GetEnvironmentVariable("AZURITE_BLOB_STORAGE") ?? throw new ArgumentException();
    jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentException();
    passwordKey = Environment.GetEnvironmentVariable("PASSWORD_KEY") ?? throw new ArgumentException();
}
else
{
    connectionString = builder.Configuration.GetConnectionString("ConnectionStrings:ProductionConnection") ?? throw new ArgumentException();
    azuriteBlobStorageConnectionString = builder.Configuration.GetSection("Azurite:BlobStorage").Value ?? throw new ArgumentException();
    jwtKey = builder.Configuration.GetSection("AuthSetting:TokenKey").Value ?? throw new ArgumentException();
    passwordKey = builder.Configuration.GetSection("AuthSetting:PasswordKey").Value ?? throw new ArgumentException();
}


var blobServiceClient = new BlobServiceClient(azuriteBlobStorageConnectionString);
var containerClient = blobServiceClient.GetBlobContainerClient("office-communicator");
await containerClient.CreateIfNotExistsAsync();


builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<OfficeDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddSingleton(sp => new DapperDbContext(connectionString));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddSingleton(sp => new BlobStorageService(containerClient));

builder.Services.AddSingleton(sp => new AuthHelper(jwtKey, passwordKey));


builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();

        policyBuilder.WithOrigins("https://0.0.0.0")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("MyPolicy", p => p
        .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        .WithOrigins("https://0.0.0.0")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
        .WithOrigins("https://10.0.2.15")
              .AllowAnyHeader()
              .AllowAnyMethod()
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
