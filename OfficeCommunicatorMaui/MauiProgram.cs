using Microsoft.Extensions.Logging;
using OfficeCommunicatorMaui.Services.API;
using OfficeCommunicatorMAUI.Services;

namespace OfficeCommunicatorMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        string serverUrl = "http://localhost:5207";

        builder.Services.AddMauiBlazorWebView();

        HttpClient httpClient = new HttpClient();
        builder.Services.AddSingleton(sp => new AuthApiService(serverUrl, httpClient));
        builder.Services.AddSingleton(sp => new GroupApiService(serverUrl, httpClient));



#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
