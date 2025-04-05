using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public class SaldoContaCorrenteRequest : IRequest<SaldoContaCorrenteResponse>
    {
        public string IdContaCorrente { get; set; } = string.Empty;
    }
}
