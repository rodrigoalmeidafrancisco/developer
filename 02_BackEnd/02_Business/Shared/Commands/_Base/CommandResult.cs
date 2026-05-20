using Developer.ExtensionCore;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Shared.Commands._Base
{
    public class CommandResult<T>
    {
        public CommandResult()
        {

        }

        public CommandResult(int status, string statusDescription, bool sucesso, string message, T data, int? total = null)
        {
            StatusCod = status;
            StatusDescricao = statusDescription;
            Success = sucesso;
            Message = message;
            Total = total;
            Data = data;
        }

        public int StatusCod { get; set; }
        public string StatusDescricao { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? Total { get; set; }
        public T Data { get; set; }
        public string Time => _countTime.Elapsed.ToString();
        [JsonIgnore]
        private readonly Stopwatch _countTime = Stopwatch.StartNew();

        public void Status200()
        {
            StatusCod = 200;
            StatusDescricao = "Requisição realizada com sucesso.";
        }

        public void Status200(string message, T data, int? total = null)
        {
            StatusCod = 200;
            StatusDescricao = "Requisição realizada com sucesso.";
            Success = true;
            Message = message;
            Data = data;
            Total = total ?? (data.IsList() ? ((IEnumerable<object>)data).Count() : 1);
        }

        public void Status400(string message)
        {
            StatusCod = 400;
            StatusDescricao = "Não foi possível processar a requisição.";
            Success = false;
            Message = message;
            Total = null;
            Data = default;
        }

        public void Status404(string message)
        {
            StatusCod = 404;
            StatusDescricao = "Dados não encontrados.";
            Success = false;
            Message = message;
            Total = null;
            Data = default;
        }

        public void Status500(Exception ex, string alternativeMessage = null)
        {
            StatusCod = 500;
            StatusDescricao = "Erro interno no servidor.";
            Success = false;
            Message = $"Ocorreu um erro inesperado: {$"<br/>- ${alternativeMessage}" ?? $" <br/>- Message: {ex.Message} <br/>- InnerException:{ex.InnerException?.Message}"}";
            Total = null;
            Data = default;
        }
    }
}
