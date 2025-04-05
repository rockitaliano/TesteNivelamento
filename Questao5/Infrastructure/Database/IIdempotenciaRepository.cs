using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;

namespace Questao5.Infrastructure.Database
{
    public interface IIdempotenciaRepository : IIdempotenciaCommandRepository, IIdempotenciaQueryRepository { }
}