using System.Text.Json.Serialization;

namespace Shared.Commands._Base
{
    public abstract class CommandBase
    {
        protected CommandBase()
        {

        }

        [JsonIgnore]
        public bool IsValid { get; set; } = false;

        [JsonIgnore]
        public List<string> ListNotifications { get; set; } = [];

        public abstract void ValidarCommand();

        public void AddNotification(string notification)
        {
            ListNotifications.Add(notification);
            IsValid = ListNotifications.Count > 0;
        }

        public CommandResult<List<string>> RetornarNotificacoes(string message = null)
        {
            CommandResult<List<string>> commandResult = new();
            commandResult.Status400(message ?? "Não foi possível processar a solicitação no servidor, pois ocorreram erros de validação", ListNotifications);
            return commandResult;
        }
    }
}
