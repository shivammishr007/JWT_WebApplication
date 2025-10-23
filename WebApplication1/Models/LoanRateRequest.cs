namespace WebApplication1.Models
{
    public class LoanRateRequest
    {
        public decimal LoanAmount { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int NumberOfPayments { get; set; }
    }
}
