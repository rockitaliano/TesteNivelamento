using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class MovimentoCommandRepository : IMovimentoCommandRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        public MovimentoCommandRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<InserirMovimentoResponse> InserirAsync(InserirMovimentoRequest request)
        {
            var response = new InserirMovimentoResponse();

            try
            {
                using var connection = new SqliteConnection(_databaseConfig.Name);
                await connection.OpenAsync();

                const string sql = @"INSERT INTO movimento 
                                    (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                                        VALUES 
                                        (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

                await connection.ExecuteAsync(sql, request.Movimento);

                response.IdMovimento = request.Movimento.IdMovimento;
                response.Sucesso = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao inserir movimento: {ex.Message}";
                return response;
            }
        }
    }
}
