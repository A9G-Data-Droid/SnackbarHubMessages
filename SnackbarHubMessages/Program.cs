using Microsoft.AspNetCore.Http.Connections;
using MudBlazor;
using MudBlazor.Services;
using SnackbarHubMessages.Data;
using SnackbarHubMessages.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSignalR(hubOptions =>
    {
        hubOptions.EnableDetailedErrors = true;
        hubOptions.MaximumParallelInvocationsPerClient = 5;
        hubOptions.StreamBufferCapacity = 16;
    })
    ;

builder.Services.AddScoped<ISnackbar, SnackbarService>();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 60000;
    config.SnackbarConfiguration.HideTransitionDuration = 555;
    config.SnackbarConfiguration.ShowTransitionDuration = 777;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<TheHub>("/hub", options =>
    {
        options.CloseOnAuthenticationExpiration = true;
        options.Transports = HttpTransportType.WebSockets;
    })
    ;

app.Run();
