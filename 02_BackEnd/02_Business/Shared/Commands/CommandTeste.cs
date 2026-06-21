using Shared.Commands._Base;

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

            if (Matricula == null)
            {
                AddNotification("A matrícula é obrigatória.");
            }
        }

    }
}
