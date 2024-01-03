using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnackbarHubMessages.Hub;
using TypedSignalR.Client;

namespace ClientHello;

public class TheHubClient : IAsyncDisposable, ITheHubClient, IHostedService
{
    private readonly HubConnection _connection;

    private readonly List<IDisposable> _subscriptions = [];

    private readonly ITheHub _strongHub;

    public TheHubClient()
    {
        _connection = ConnectionToTheHub();

        _strongHub = _connection.CreateHubProxy<ITheHub>();

        // All client methods are magically added here with strong names
        _subscriptions.Add(_connection.Register<ITheHubClient>(this));

    }

    private static HubConnection ConnectionToTheHub()
    {
        return new HubConnectionBuilder()
            .WithUrl("http://localhost:5171/hub")
            .ConfigureLogging(logging =>
            {
                logging
                    .AddConsole()
                    .AddDebug()
                    ;
            })
            .WithAutomaticReconnect([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(33)])
            .WithStatefulReconnect()
            .Build();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try  // To make initial connection
            {
                await ConnectAsync(cancellationToken).ConfigureAwait(false);
                await _strongHub.SayToServer("Hello").ConfigureAwait(false);

                return;
            }
            catch (Exception e)
            {
                // Don't allow the hub to crash the whole program. Log and move on.
                Console.WriteLine(e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(11), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _strongHub.SayToServer("Goodbye").ConfigureAwait(false);

        await DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Calls <see cref="HubConnection.StartAsync"/>
    /// </summary>
    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        // StartAsync will throw if it's not disconnected. 
        if (_connection.State != HubConnectionState.Disconnected) return Task.CompletedTask;

        return _connection.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // Dispose of unmanaged resources.
        await DisposeAsyncCore().ConfigureAwait(false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        _subscriptions.ForEach(thing => thing.Dispose());

        await _connection.DisposeAsync().ConfigureAwait(false);
    }
}