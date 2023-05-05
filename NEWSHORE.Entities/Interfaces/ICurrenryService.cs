namespace NEWSHORE.Entities.Interfaces
{
    public interface ICurrenryService
    {
        Task<double> Get( double Amount, string to = "USD", string from = "USD");
    }
}
