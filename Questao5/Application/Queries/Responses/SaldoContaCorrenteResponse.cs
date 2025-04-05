namespace Questao5.Application.Queries.Responses
{
    public class SaldoContaCorrenteResponse
    {
        public int? Numero { get; set; }
        public string? Nome { get; set; }
        public string? DataHoraConsulta { get; set; }
        public decimal Saldo { get; set; }
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public string? TipoErro { get; set; }
    }
}
