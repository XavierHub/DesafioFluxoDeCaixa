namespace EmpXpo.Accounting.Domain.Exceptions
{
    public class CashFlowException : Exception
    {
        public CashFlowException(string mensagem)
           : base(mensagem) { }
    }
}
