using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class IdempotenciaCommandRepository : IIdempotenciaCommandRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        public IdempotenciaCommandRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
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
