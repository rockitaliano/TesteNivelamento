using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class ContaCorrenteCommandRepository : IContaCorrenteCommandRepository
    {
        private readonly IContaCorrenteQueryRepository _queryRepository;
        public ContaCorrenteCommandRepository(DatabaseConfig databaseConfig)
        {
            _queryRepository = new ContaCorrenteQueryRepository(databaseConfig);
        }

        public async Task<ContaCorrente?> ObterPorIdAsync(string idContaCorrente)
        {
            return await _queryRepository.ObterPorIdAsync(idContaCorrente);
        }

        public async Task<ContaCorrente?> ObterPorNumeroAsync(int numero)
        {
            return await _queryRepository.ObterPorNumeroAsync(numero);
        }
    }
}
