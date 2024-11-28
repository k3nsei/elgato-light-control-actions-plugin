namespace ElgatoLightApiClient.Commands
{
    internal interface ICommand { }

    internal interface ICommand<out TResult> { }
}