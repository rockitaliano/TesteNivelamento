using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Questao1
{
    public class ContaBancaria
    {
        private string _numero;

        public string Numero
        {
            get { return _numero; }
            private set
            {
                if (!Regex.IsMatch(value, @"^[0-9-]+$"))
                {
                    throw new ArgumentException("O número da conta deve conter apenas números e hífens.");
                }
                _numero = value;
            }
        }
        public string Titular { get; set; }
        public double Saldo { get; private set; }

        private const double TaxaSaque = 3.50;

        public ContaBancaria(string numero, string titular, double depositoInicial = 0.0)
        {
            Numero = numero;
            Titular = titular;
            Saldo = depositoInicial;
        }

        public void Deposito(double quantia)
        {
            Saldo += quantia;
        }

        public void Saque(double quantia)
        {
            // Aplica a taxa de saque independentemente do saldo disponível, pode ficar negativo            
            Saldo -= (quantia + TaxaSaque);
        }
        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
