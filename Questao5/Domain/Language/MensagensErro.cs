using Questao5.Domain.Enumerators;

namespace Questao5.Domain.Language
{
    public static class MensagensErro
    {
        public static string ObterMensagem(TipoErro tipoErro)
        {
            return tipoErro switch
            {
                TipoErro.INVALID_ACCOUNT => "Conta corrente não encontrada.",
                TipoErro.INACTIVE_ACCOUNT => "Conta corrente não está ativa.",
                TipoErro.INVALID_VALUE => "Valor da movimentação deve ser maior que zero.",
                TipoErro.INVALID_TYPE => "Tipo de movimentação inválido. Utilize 'C' para crédito ou 'D' para débito.",
                _ => "Erro não identificado."
            };
        }
    }
}
