using FluentAssertions;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using Xunit;

namespace Questao5.Infrastructure.Tests.Application.Handlers
{
    public class MovimentacaoContaCorrenteHandlerTests
    {
        private readonly IContaCorrenteCommandRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly MovimentacaoContaCorrenteHandler _handler;

        public MovimentacaoContaCorrenteHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteCommandRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _idempotenciaRepository = Substitute.For<IIdempotenciaRepository>();

            _handler = new MovimentacaoContaCorrenteHandler(
                _contaCorrenteRepository,
                _movimentoRepository,
                _idempotenciaRepository);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteNaoExiste_DeveRetornarErroContaInvalida()
        {
            // Arrange
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "1",
                TipoMovimento = "C",
                Valor = 100
            };

            //  o mock para retornar null conta não existe
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns((ContaCorrente)null);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.TipoErro.Should().Be(TipoErro.INVALID_ACCOUNT.ToString());
            response.Mensagem.Should().Be(MensagensErro.ObterMensagem(TipoErro.INVALID_ACCOUNT));
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteInativa_DeveRetornarErroContaInativa()
        {
            // Arrange
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "1",
                TipoMovimento = "C",
                Valor = 100
            };

            // mock para retornar uma conta inativa
            var contaInativa = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: false
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaInativa);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.TipoErro.Should().Be(TipoErro.INACTIVE_ACCOUNT.ToString());
            response.Mensagem.Should().Be(MensagensErro.ObterMensagem(TipoErro.INACTIVE_ACCOUNT));
        }

        [Fact]
        public async Task Handle_QuandoValorInvalido_DeveRetornarErroValorInvalido()
        {
            // Arrange
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "1",
                TipoMovimento = "C",
                Valor = 0 // Valor inválido
            };

            // Mock para retornar uma conta ativa
            var contaAtiva = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: true
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaAtiva);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.TipoErro.Should().Be(TipoErro.INVALID_VALUE.ToString());
            response.Mensagem.Should().Be(MensagensErro.ObterMensagem(TipoErro.INVALID_VALUE));
        }

        [Fact]
        public async Task Handle_QuandoTipoMovimentoInvalido_DeveRetornarErroTipoInvalido()
        {
            // Arrange
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "1",
                TipoMovimento = "X", // Tipo inválido
                Valor = 100
            };

            // Mock para retornar uma conta ativa
            var contaAtiva = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: true
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaAtiva);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.TipoErro.Should().Be(TipoErro.INVALID_TYPE.ToString());
            response.Mensagem.Should().Be(MensagensErro.ObterMensagem(TipoErro.INVALID_TYPE));
        }

        [Fact]
        public async Task Handle_QuandoMovimentacaoValida_DeveRetornarSucesso()
        {
            // Arrange
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "1",
                TipoMovimento = "C",
                Valor = 100
            };

            var contaAtiva = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: true
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaAtiva);

            // Configurando o mock para simular a inserção do movimento
            var idMovimento = Guid.NewGuid().ToString();
            _movimentoRepository.InserirAsync(Arg.Any<InserirMovimentoRequest>())
                .Returns(new InserirMovimentoResponse { IdMovimento = idMovimento, Sucesso = true });

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeTrue();
            response.IdMovimento.Should().Be(idMovimento);
        }

        [Fact]
        public async Task Handle_QuandoRequisicaoIdempotente_DeveRetornarResultadoArmazenado()
        {
            // Arrange
            var chaveIdempotencia = Guid.NewGuid().ToString();
            var request = new MovimentacaoContaCorrenteRequest
            {
                ChaveIdempotencia = chaveIdempotencia,
                IdContaCorrente = "1",
                TipoMovimento = "C",
                Valor = 100
            };

            // Criando uma resposta armazenada para simular idempotência
            var respostaArmazenada = new MovimentacaoContaCorrenteResponse
            {
                IdMovimento = Guid.NewGuid().ToString(),
                Sucesso = true
            };

            // Mock para retornar uma requisição idempotente
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = chaveIdempotencia,
                Requisicao = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                Resultado = Newtonsoft.Json.JsonConvert.SerializeObject(respostaArmazenada)
            };
            _idempotenciaRepository.ObterPorChaveAsync(chaveIdempotencia).Returns(idempotencia);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeTrue();
            response.IdMovimento.Should().Be(respostaArmazenada.IdMovimento);

            // Verificar que não houve com o repositório de conta corrente
            await _contaCorrenteRepository.DidNotReceive().ObterPorIdAsync(Arg.Any<string>());
        }
    }
}
