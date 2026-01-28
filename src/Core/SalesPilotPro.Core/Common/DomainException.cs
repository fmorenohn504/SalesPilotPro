namespace SalesPilotPro.Core.Common;

// Excepción base del dominio
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
