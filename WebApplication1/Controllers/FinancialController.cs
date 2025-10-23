using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialController : ControllerBase
    {
        [HttpPost("loan-rate")]
        public ActionResult<LoanRateResponse> CalculateLoanRate([FromBody] LoanRateRequest req)
        {
            if (req.LoanAmount <= 0 || req.MonthlyPayment <= 0 || req.NumberOfPayments <= 0)
                return BadRequest("Invalid input");

            // Newton-Raphson for monthly rate
            decimal pv = req.LoanAmount;
            decimal pmt = req.MonthlyPayment;
            int n = req.NumberOfPayments;
            double r = 0.05 / 12; // initial guess
            for (int i = 0; i < 100; i++)
            {
                double f = (double)pmt * (1 - Math.Pow(1 + r, -n)) / r - (double)pv;
                double fp = (double)pmt * (Math.Pow(1 + r, -n) * n / (1 + r) / r - (1 - Math.Pow(1 + r, -n)) / (r * r));
                double newR = r - f / fp;
                if (Math.Abs(newR - r) < 1e-8) break;
                r = newR;
            }
            decimal annualRate = (decimal)(Math.Pow(1 + r, 12) - 1) * 100;
            return Ok(new LoanRateResponse { AnnualRate = Math.Round(annualRate, 2) });
        }

        [HttpPost("cagr")]
        public ActionResult<CagrResponse> CalculateCagr([FromBody] CagrRequest req)
        {
            if (req.BeginningValue <= 0 || req.EndingValue <= 0 || req.Years <= 0)
                return BadRequest("Invalid input");
            double cagr = Math.Pow((double)req.EndingValue / (double)req.BeginningValue, 1.0 / req.Years) - 1;
            return Ok(new CagrResponse { Cagr = Math.Round((decimal)(cagr * 100), 2) });
        }

        [HttpPost("sip")]
        public ActionResult<SipResponse> CalculateSip([FromBody] SipRequest req)
        {
            if (req.MonthlyInvestment <= 0 || req.ExpectedAnnualReturn <= 0 || req.InvestmentPeriodInYears <= 0)
                return BadRequest("Invalid input");
            int n = req.InvestmentPeriodInYears * 12;
            double ra = (double)req.ExpectedAnnualReturn / 100.0;
            double rm = Math.Pow(1 + ra, 1.0 / 12) - 1;
            double fv = (double)req.MonthlyInvestment * (Math.Pow(1 + rm, n) - 1) / rm * (1 + rm);
            return Ok(new SipResponse { FutureValue = Math.Round((decimal)fv, 2) });
        }
    }
}
