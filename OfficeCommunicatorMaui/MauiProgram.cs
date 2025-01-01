using Microsoft.Extensions.Logging;
using OfficeCommunicatorMaui.Services;
using OfficeCommunicatorMaui.Services.API;
using OfficeCommunicatorMaui.Services.Repositories;

namespace OfficeCommunicatorMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            string serverUrl = "http://localhost:5207";
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app_database.db");
            Random random = new Random();

            builder.Services.AddMauiBlazorWebView();

            HttpClient httpClient = new HttpClient();
            SqliteDbContext dbContext = new SqliteDbContext(dbPath);

            builder.Services.AddSingleton(sp => new AuthApiService(serverUrl, httpClient));
            builder.Services.AddSingleton(sp => new ContactApiService(serverUrl, httpClient));
            builder.Services.AddSingleton(sp => new GroupApiService(serverUrl, httpClient));
            builder.Services.AddSingleton(sp => new ChatApiService(serverUrl, httpClient));

            builder.Services.AddSingleton(sp => dbContext);

            builder.Services.AddSingleton(sp => new MessageRepository(dbContext));

            builder.Services.AddSingleton(sp => new SecureStorageService(random.Next(1, 3)));

            builder.Services.AddBlazorBootstrap();




#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif


            MauiApp mauiApp = builder.Build();

            using (var scope = mauiApp.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                context.Database.EnsureCreated();
            }

            return mauiApp;

        }
    }
}
