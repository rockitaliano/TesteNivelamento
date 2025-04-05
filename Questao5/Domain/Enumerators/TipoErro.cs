namespace Questao5.Domain.Enumerators
{
    public enum TipoErro
    {
        INVALID_ACCOUNT,     // Conta corrente não cadastrada
        INACTIVE_ACCOUNT,    // Conta corrente inativa
        INVALID_VALUE,      // Valor inválido (não positivo)
        INVALID_TYPE         // Tipo de movimento inválido (diferente de C ou D)
    }
}
