namespace Questao5.Infrastructure.Database.CommandStore.Responses
{
    public class InserirMovimentoResponse
    {
        public string IdMovimento { get; set; } = string.Empty;

        public bool Sucesso { get; set; }

        public string? Mensagem { get; set; }
    }
}
