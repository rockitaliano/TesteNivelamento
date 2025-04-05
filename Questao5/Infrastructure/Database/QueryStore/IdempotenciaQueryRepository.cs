using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class IdempotenciaQueryRepository : IIdempotenciaQueryRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public IdempotenciaQueryRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<Idempotencia?> ObterPorChaveAsync(string chaveIdempotencia)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.OpenAsync();

            const string sql = @"SELECT 
                                    chave_idempotencia as ChaveIdempotencia, 
                                    requisicao as Requisicao, 
                                    resultado as Resultado 
                                FROM idempotencia 
                                WHERE chave_idempotencia = @ChaveIdempotencia";

            var idempotencia = await connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { ChaveIdempotencia = chaveIdempotencia });

            return idempotencia;
        }

        public async Task SalvarAsync(Idempotencia idempotencia)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            await connection.OpenAsync();

            const string sql = @"INSERT INTO idempotencia 
                                (chave_idempotencia, requisicao, resultado) 
                                VALUES 
                                (@ChaveIdempotencia, @Requisicao, @Resultado)";

            await connection.ExecuteAsync(sql, idempotencia);
        }
    }
}
