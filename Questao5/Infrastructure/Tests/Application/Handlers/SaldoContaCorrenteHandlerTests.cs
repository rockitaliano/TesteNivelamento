using FluentAssertions;
using NSubstitute;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Language;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using Xunit;

namespace Questao5.Infrastructure.Tests.Application.Handlers
{
    public class SaldoContaCorrenteHandlerTests
    {
        private readonly IContaCorrenteCommandRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly SaldoContaCorrenteHandler _handler;

        public SaldoContaCorrenteHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteCommandRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();


            _handler = new SaldoContaCorrenteHandler(
                _contaCorrenteRepository,
                _movimentoRepository);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteNaoExiste_DeveRetornarErroContaInvalida()
        {
            // Arrange
            var request = new SaldoContaCorrenteRequest
            {
                IdContaCorrente = "1"
            };

            // Configurando o mock para retornar null (conta não existe)
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
            var request = new SaldoContaCorrenteRequest
            {
                IdContaCorrente = "1"
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
        public async Task Handle_QuandoConsultaSaldoValida_DeveRetornarSaldoCorreto()
        {
            // Arrange
            var request = new SaldoContaCorrenteRequest
            {
                IdContaCorrente = "1"
            };

            //mock para retornar uma conta ativa
            var contaAtiva = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: true
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaAtiva);

            // mock para retornar um saldo
            var saldoResponse = new ObterSaldoResponse
            {
                Sucesso = true,
                Saldo = 1500.50m
            };
            _movimentoRepository.ObterSaldoAsync(Arg.Any<ObterSaldoRequest>()).Returns(saldoResponse);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeTrue();
            response.Numero.Should().Be(contaAtiva.Numero);
            response.Nome.Should().Be(contaAtiva.Nome);
            response.Saldo.Should().Be(saldoResponse.Saldo);
            response.DataHoraConsulta.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_QuandoErroAoObterSaldo_DeveRetornarErro()
        {
            // Arrange
            var request = new SaldoContaCorrenteRequest
            {
                IdContaCorrente = "1"
            };

            //  mock para retornar uma conta ativa
            var contaAtiva = new ContaCorrente
            (
                idContaCorrente: "1",
                numero: 123,
                nome: "Conta Teste",
                ativo: true
            );
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente).Returns(contaAtiva);

            // Configurando o mock para retornar erro ao obter saldo
            var saldoResponse = new ObterSaldoResponse
            {
                Sucesso = false,
                Mensagem = "Erro ao calcular saldo"
            };
            _movimentoRepository.ObterSaldoAsync(Arg.Any<ObterSaldoRequest>()).Returns(saldoResponse);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.Mensagem.Should().Be(saldoResponse.Mensagem);
        }

        [Fact]
        public async Task Handle_QuandoExcecaoOcorre_DeveRetornarErro()
        {
            // Arrange
            var request = new SaldoContaCorrenteRequest
            {
                IdContaCorrente = "1"
            };

            // Mock para lançar uma exceção
            _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente)
                .Returns(Task.FromException<ContaCorrente>(new Exception("Erro simulado")));

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Sucesso.Should().BeFalse();
            response.Mensagem.Should().Contain("Erro ao consultar o saldo");
            response.Mensagem.Should().Contain("Erro simulado");
        }
    }
}
