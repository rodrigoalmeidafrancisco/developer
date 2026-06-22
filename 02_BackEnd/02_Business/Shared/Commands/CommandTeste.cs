using Shared.Commands._Base;
using Shared.Notifications.Validations;

namespace Shared.Commands
{
    public class CommandTeste : CommandBaseCrud
    {
        public CommandTeste()
        {

        }

        public int? Matricula { get; set; }

        public override void ValidarCommand()
        {
            base.ValidarCommand();

            AddNotifications(new Contract<CommandTeste>().Requires()
                .IsNull(Matricula, "A matrícula é obrigatória.")
            );
        }

    }
}
