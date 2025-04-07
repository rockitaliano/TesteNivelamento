using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IMovimentoCommandRepository _commandRepository;
        private readonly IMovimentoQueryRepository _queryRepository;

        public MovimentoRepository(DatabaseConfig databaseConfig)
        {
            _commandRepository = new MovimentoCommandRepository(databaseConfig);
            _queryRepository = new MovimentoQueryRepository(databaseConfig);
        }

        public async Task<InserirMovimentoResponse> InserirAsync(InserirMovimentoRequest request)
        {
            return await _commandRepository.InserirAsync(request);
        }

        public async Task<string> InserirAsync(Movimento movimento)
        {
            var request = new InserirMovimentoRequest { Movimento = movimento };
            var response = await InserirAsync(request);
            return response.IdMovimento;
        }

        public async Task<ObterSaldoResponse> ObterSaldoAsync(ObterSaldoRequest request)
        {
            return await _queryRepository.ObterSaldoAsync(request);
        }

        public async Task<decimal> ObterSaldoAsync(string idContaCorrente)
        {
            var request = new ObterSaldoRequest { IdContaCorrente = idContaCorrente };
            var response = await ObterSaldoAsync(request);
            return response.Saldo;
        }
    }
}