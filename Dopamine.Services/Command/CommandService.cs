using CoreWCF;
using Dopamine.Core.Prism;

namespace Dopamine.Services.Command
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommandService : ICommandService
    {
        public void ShowMainWindowCommand()
        {
            ApplicationCommands.ShowMainWindowCommand.Execute(null);
        }
    }
}
