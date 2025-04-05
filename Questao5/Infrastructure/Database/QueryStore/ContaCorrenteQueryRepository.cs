using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class ContaCorrenteQueryRepository : IContaCorrenteQueryRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public ContaCorrenteQueryRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<ContaCorrente?> ObterPorIdAsync(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.OpenAsync();

            const string sql = @"SELECT 
                                    idcontacorrente as IdContaCorrente, 
                                    numero as Numero, 
                                    nome as Nome, 
                                    ativo as Ativo 
                                FROM contacorrente 
                                WHERE idcontacorrente = @IdContaCorrente";

            var contaCorrente = await connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { IdContaCorrente = idContaCorrente });

            return contaCorrente;
        }

        public async Task<ContaCorrente?> ObterPorNumeroAsync(int numero)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.OpenAsync();

            const string sql = @"SELECT 
                                    idcontacorrente as IdContaCorrente, 
                                    numero as Numero, 
                                    nome as Nome, 
                                    ativo as Ativo 
                                FROM contacorrente 
                                WHERE numero = @Numero";

            var contaCorrente = await connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Numero = numero });

            return contaCorrente;
        }
    }
}
