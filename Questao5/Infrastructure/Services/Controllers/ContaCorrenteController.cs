using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza uma movimentação (crédito ou débito) em uma conta corrente
        /// </summary>
        /// <param name="request">Dados da movimentação</param>
        /// <returns>Resultado da operação com o ID do movimento gerado</returns>
        /// <response code="200">Retorna o resultado da operação</response>
        /// <response code="400">Se ocorrer algum erro de validação</response>
        /// <response code="500">Se ocorrer algum erro interno no servidor</response>
        [HttpPost("movimentacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RealizarMovimentacao([FromBody] MovimentacaoContaCorrenteRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Consulta o saldo atual de uma conta corrente
        /// </summary>
        /// <param name="idContaCorrente">ID da conta corrente</param>
        /// <returns>Saldo atual da conta corrente</returns>
        /// <response code="200">Retorna o saldo da conta corrente</response>
        /// <response code="400">Se ocorrer algum erro de validação</response>
        /// <response code="500">Se ocorrer algum erro interno no servidor</response>
        [HttpGet("saldo/{idContaCorrente}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
        {
            var request = new SaldoContaCorrenteRequest { IdContaCorrente = idContaCorrente };
            var response = await _mediator.Send(request);

            if (!response.Sucesso)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
