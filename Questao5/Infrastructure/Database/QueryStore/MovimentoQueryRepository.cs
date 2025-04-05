using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class MovimentoQueryRepository : IMovimentoQueryRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        public MovimentoQueryRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<ObterSaldoResponse> ObterSaldoAsync(ObterSaldoRequest request)
        {
            var response = new ObterSaldoResponse();

            try
            {
                using var connection = new SqliteConnection(_databaseConfig.Name);
                await connection.OpenAsync();

                // Consulta para obter a soma dos créditos
                const string sqlCreditos = @"SELECT COALESCE(SUM(valor), 0) 
                                            FROM movimento 
                                                WHERE idcontacorrente = @IdContaCorrente 
                                                AND tipomovimento = 'C'";

                // Consulta para obter a soma dos débitos
                const string sqlDebitos = @"SELECT COALESCE(SUM(valor), 0) 
                                           FROM movimento 
                                           WHERE idcontacorrente = @IdContaCorrente 
                                           AND tipomovimento = 'D'";

                var creditos = await connection.ExecuteScalarAsync<decimal>(sqlCreditos, new { IdContaCorrente = request.IdContaCorrente });
                var debitos = await connection.ExecuteScalarAsync<decimal>(sqlDebitos, new { IdContaCorrente = request.IdContaCorrente });

                // Cálculo do saldo: SOMA_DOS_CREDITOS - SOMA_DOS_DEBITOS
                response.Saldo = creditos - debitos;
                response.Sucesso = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao obter saldo: {ex.Message}";
                return response;
            }
        }
    }
}
