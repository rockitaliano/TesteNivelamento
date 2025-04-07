namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        private int _numero;
        public string IdContaCorrente { get; set; } = string.Empty;
        public int Numero
        {
            get => _numero;
            internal set => _numero = value;
        }
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        public ContaCorrente(string idContaCorrente, int numero, string nome, bool ativo)
        {
            IdContaCorrente = idContaCorrente;
            _numero = numero;
            Nome = nome;
            Ativo = ativo;
        }

        public ContaCorrente()
        {
            IdContaCorrente = string.Empty;
            Nome = string.Empty;
        }

    }
}
