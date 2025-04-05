using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public interface IIdempotenciaQueryRepository
    {
        Task<Idempotencia?> ObterPorChaveAsync(string chaveIdempotencia);
    }
}
