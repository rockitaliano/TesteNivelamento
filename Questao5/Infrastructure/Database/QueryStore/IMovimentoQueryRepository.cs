using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public interface IMovimentoQueryRepository
    {
        Task<ObterSaldoResponse> ObterSaldoAsync(ObterSaldoRequest request);
    }

}
