using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IIdempotenciaCommandRepository _commandRepository;
        private readonly IIdempotenciaQueryRepository _queryRepository;

        public IdempotenciaRepository(DatabaseConfig databaseConfig)
        {
            _commandRepository = new IdempotenciaCommandRepository(databaseConfig);
            _queryRepository = new IdempotenciaQueryRepository(databaseConfig);
        }

        public async Task<Idempotencia?> ObterPorChaveAsync(string chaveIdempotencia)
        {
            return await _queryRepository.ObterPorChaveAsync(chaveIdempotencia);
        }

        public async Task SalvarAsync(Idempotencia idempotencia)
        {
            await _commandRepository.SalvarAsync(idempotencia);
        }
    }
}