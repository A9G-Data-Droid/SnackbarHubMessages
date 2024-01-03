using Microsoft.AspNetCore.SignalR;
using MudBlazor;

namespace SnackbarHubMessages.Hub;

public class TheHub : Hub<ITheHubClient>, ITheHub
{
    private readonly ISnackbar _snackBar;

    /// <summary>
    /// Called in <see cref="Program.Main"/> to MapHub.
    /// </summary>
    /// <param name="snackBar">Send messages to the user.</param>
    public TheHub(ISnackbar snackBar)
    {
        _snackBar = snackBar;
    }

    /// <summary>
    /// This runs each time a client connects, track them in a list!
    /// </summary>
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("Pretend I'm doing something here...");

        return base.OnConnectedAsync();
    }

    /// <summary>
    /// This runs when a client disconnects. Stop tracking them!
    /// </summary>
    /// <param name="exception">We should log this.</param>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("Pretend I'm doing something here...");

        return base.OnDisconnectedAsync(exception);
    }

    /// <inheritdoc />
    public Task SayToServer(string message)
    {
        _snackBar.Add(message, Severity.Success);

        return Task.CompletedTask;
    }
}