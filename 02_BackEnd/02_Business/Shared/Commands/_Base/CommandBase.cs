using Shared.Notifications;

namespace Shared.Commands._Base
{
    public abstract class CommandBase : Notifiable<Notification>
    {
        protected CommandBase()
        {

        }

        public abstract void ValidarCommand();

        public CommandResult<List<string>> RetornarNotificacoes(string message = null)
        {
            CommandResult<List<string>> commandResult = new();
            commandResult.Status400(message ?? "Não foi possível processar a solicitação no servidor, pois ocorreram erros de validação", GetNotificationsMessages());
            return commandResult;
        }
    }
}
