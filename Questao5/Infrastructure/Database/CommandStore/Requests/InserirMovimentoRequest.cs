using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.CommandStore.Requests
{
    public class InserirMovimentoRequest
    {
        public Movimento Movimento { get; set; } = new Movimento();
    }
}
