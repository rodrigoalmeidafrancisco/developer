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

        public CommandResult(int status, string message, T data, int? total = null)
        {
            StatusCod = status;
            Message = message;
            Total = total;
            Data = data;
        }

        public int StatusCod { get; set; }
        public string Message { get; set; }
        public int? Total { get; set; }
        public T Data { get; set; }
        public string Time => _countTime.Elapsed.ToString();
        [JsonIgnore]
        private readonly Stopwatch _countTime = Stopwatch.StartNew();

        public void Status200()
        {
            StatusCod = 200;
        }

        public void Status200(string message, T data, int? total = null)
        {
            StatusCod = 200;
            Message = message;
            Data = data;
            Total = total ?? (data.IsList() ? ((IEnumerable<object>)data).Count() : 1);
        }

        public void Status400(string message)
        {
            StatusCod = 400;
            Message = message;
            Total = null;
            Data = default;
        }

        public void Status400(string message, T data)
        {
            StatusCod = 400;
            Message = message;
            Total = null;
            Data = data;
        }

        public void Status404(string message)
        {
            StatusCod = 404;
            Message = message;
            Total = null;
            Data = default;
        }

        public void Status500(Exception ex, string alternativeMessage = null)
        {
            StatusCod = 500;
            Message = $"Ocorreu um erro inesperado: {$"<br/>- ${alternativeMessage}" ?? $" <br/>- Message: {ex.Message} <br/>- InnerException:{ex.InnerException?.Message}"}";
            Total = null;
            Data = default;
        }


    }
}
