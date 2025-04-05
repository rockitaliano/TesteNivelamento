using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.CommandStore.Requests;

namespace Questao5.Application.Handlers
{
    public class MovimentacaoContaCorrenteHandler : IRequestHandler<MovimentacaoContaCorrenteRequest, MovimentacaoContaCorrenteResponse>
    {
        private readonly IContaCorrenteCommandRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentacaoContaCorrenteHandler(
            IContaCorrenteCommandRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<MovimentacaoContaCorrenteResponse> Handle(MovimentacaoContaCorrenteRequest request, CancellationToken cancellationToken)
        {
            // Verificar idempotência
            var idempotencia = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
            if (idempotencia != null)
            {
                // Se a requisição já foi processada, retorna o resultado armazenado
                if (!string.IsNullOrEmpty(idempotencia.Resultado))
                    return JsonConvert.DeserializeObject<MovimentacaoContaCorrenteResponse>(idempotencia.Resultado)!;

            }

            var response = new MovimentacaoContaCorrenteResponse();

            try
            {
                // Valida a conta
                var contaCorrente = await _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente);
                if (contaCorrente == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INVALID_ACCOUNT);
                    response.TipoErro = TipoErro.INVALID_ACCOUNT.ToString();
                    await SalvarIdempotencia(request, response);
                    return response;
                }

                // Validar se a conta está ativa
                if (!contaCorrente.Ativo)
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INACTIVE_ACCOUNT);
                    response.TipoErro = TipoErro.INACTIVE_ACCOUNT.ToString();
                    await SalvarIdempotencia(request, response);
                    return response;
                }

                // Validar valor
                if (request.Valor <= 0)
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INVALID_VALUE);
                    response.TipoErro = TipoErro.INVALID_VALUE.ToString();
                    await SalvarIdempotencia(request, response);
                    return response;
                }

                // Validar tipo de movimento
                if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INVALID_TYPE);
                    response.TipoErro = TipoErro.INVALID_TYPE.ToString();
                    await SalvarIdempotencia(request, response);
                    return response;
                }

                // Criar movimento
                var movimento = new Movimento
                {
                    IdMovimento = Guid.NewGuid().ToString(),
                    IdContaCorrente = request.IdContaCorrente,
                    DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
                    TipoMovimento = request.TipoMovimento,
                    Valor = request.Valor
                };

                // Inserir movimento usando o novo padrão de Request/Response
                var movimentoRequest = new InserirMovimentoRequest { Movimento = movimento };
                var movimentoResponse = await _movimentoRepository.InserirAsync(movimentoRequest);

                // Verificar se a operação foi bem-sucedida
                if (!movimentoResponse.Sucesso)
                {
                    response.Sucesso = false;
                    response.Mensagem = movimentoResponse.Mensagem ?? "Erro ao inserir movimento";
                    await SalvarIdempotencia(request, response);
                    return response;
                }

                var idMovimento = movimentoResponse.IdMovimento;

                // Preparar resposta de sucesso
                response.Sucesso = true;
                response.IdMovimento = idMovimento;

                // Salvar idempotência
                await SalvarIdempotencia(request, response);

                return response;
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao processar a movimentação: {ex.Message}";
                await SalvarIdempotencia(request, response);
                return response;
            }
        }

        private async Task SalvarIdempotencia(MovimentacaoContaCorrenteRequest request, MovimentacaoContaCorrenteResponse response)
        {
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                Requisicao = JsonConvert.SerializeObject(request),
                Resultado = JsonConvert.SerializeObject(response)
            };

            await _idempotenciaRepository.SalvarAsync(idempotencia);
        }
    }
}
