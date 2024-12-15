using Microsoft.Extensions.Logging;
using OfficeCommunicatorMaui.Services;
using OfficeCommunicatorMaui.Services.API;
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
        builder.Services.AddSingleton(sp => new ContactApiService(serverUrl, httpClient));
        builder.Services.AddSingleton(sp => new ChatApiService(serverUrl, httpClient));
        builder.Services.AddSingleton(sp => new SignalRService(serverUrl + "/chatHub"));



#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
