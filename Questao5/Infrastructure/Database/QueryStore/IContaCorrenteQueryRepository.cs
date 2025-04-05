using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public interface IContaCorrenteQueryRepository
    {
        Task<ContaCorrente?> ObterPorIdAsync(string idContaCorrente);
        Task<ContaCorrente?> ObterPorNumeroAsync(int numero);
    }
}
