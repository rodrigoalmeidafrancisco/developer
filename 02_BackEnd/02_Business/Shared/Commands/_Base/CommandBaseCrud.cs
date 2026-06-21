using System.Text.Json.Serialization;

namespace Shared.Commands._Base
{
    public class CommandBaseCrud : CommandBase
    {
        public CommandBaseCrud()
        {

        }

        [JsonIgnore]
        public string UsuarioLog { get; set; }

        [JsonIgnore]
        public List<string> UsuarioRoles { get; set; }

        public override void ValidarCommand()
        {
            if (string.IsNullOrEmpty(UsuarioLog))
            {
                AddNotification("O usuário logado é obrigatório.");
            }

            if (UsuarioRoles == null || UsuarioRoles.Count == 0)
            {
                AddNotification("O usuário logado deve ter pelo menos uma função.");
            }
        }
    }
}
