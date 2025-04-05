using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    public class MovimentacaoContaCorrenteRequest : IRequest<MovimentacaoContaCorrenteResponse>
    {
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
    }
}
