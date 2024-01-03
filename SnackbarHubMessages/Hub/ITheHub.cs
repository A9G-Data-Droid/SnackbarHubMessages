
namespace SnackbarHubMessages.Hub;

/// <summary>
/// Strongly typed hub definition for TheHub.
/// </summary>
public interface ITheHub
{
    /// <summary>
    /// Receiver status changes.
    /// </summary>
    /// <param name="message">What says the client?</param>
    public Task SayToServer(string message);

}