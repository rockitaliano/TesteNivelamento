namespace Questao5.Infrastructure.Database.QueryStore.Responses
{
    public class ObterSaldoResponse
    {
        public decimal Saldo { get; set; }
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
    }
}
