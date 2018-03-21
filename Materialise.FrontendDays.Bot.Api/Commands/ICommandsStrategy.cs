namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public interface ICommandsStrategy
    {
        ICommand Resolve(string command);
    }
}