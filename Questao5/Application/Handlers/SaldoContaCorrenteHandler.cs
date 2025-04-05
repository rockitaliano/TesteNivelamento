using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore.Requests;

namespace Questao5.Application.Handlers
{
    public class SaldoContaCorrenteHandler : IRequestHandler<SaldoContaCorrenteRequest, SaldoContaCorrenteResponse>
    {
        private readonly IContaCorrenteCommandRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public SaldoContaCorrenteHandler(
            IContaCorrenteCommandRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<SaldoContaCorrenteResponse> Handle(SaldoContaCorrenteRequest request, CancellationToken cancellationToken)
        {
            var response = new SaldoContaCorrenteResponse();

            try
            {
                var contaCorrente = await _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente);
                if (contaCorrente == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INVALID_ACCOUNT);
                    response.TipoErro = TipoErro.INVALID_ACCOUNT.ToString();
                    return response;
                }

                if (!contaCorrente.Ativo)
                {
                    response.Sucesso = false;
                    response.Mensagem = MensagensErro.ObterMensagem(TipoErro.INACTIVE_ACCOUNT);
                    response.TipoErro = TipoErro.INACTIVE_ACCOUNT.ToString();
                    return response;
                }

                var saldoRequest = new ObterSaldoRequest { IdContaCorrente = request.IdContaCorrente };
                var saldoResponse = await _movimentoRepository.ObterSaldoAsync(saldoRequest);

                if (!saldoResponse.Sucesso)
                {
                    response.Sucesso = false;
                    response.Mensagem = saldoResponse.Mensagem ?? "Erro ao obter saldo";
                    return response;
                }

                response.Sucesso = true;
                response.Numero = contaCorrente.Numero;
                response.Nome = contaCorrente.Nome;
                response.DataHoraConsulta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                response.Saldo = saldoResponse.Saldo;

                return response;
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao consultar o saldo: {ex.Message}";
                return response;
            }
        }
    }
}
