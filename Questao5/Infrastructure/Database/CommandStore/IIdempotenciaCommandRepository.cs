using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public interface IIdempotenciaCommandRepository
    {
        Task SalvarAsync(Idempotencia idempotencia);
    }
}
