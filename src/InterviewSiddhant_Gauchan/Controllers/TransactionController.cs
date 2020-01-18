using InterviewSiddhant_Gauchan.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionHandler transactionsHandler;
        private readonly ITransactionSummaryHandler transactionSummaryHandler;

        public TransactionController(ITransactionHandler transactionsHandler,ITransactionSummaryHandler transactionSummaryHandler)
        {
            this.transactionsHandler = transactionsHandler;
            this.transactionSummaryHandler = transactionSummaryHandler;
        }
        
        [HttpGet]
        [Route("transactions")]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            try
            {
                var result = await transactionsHandler.Get();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
                
        [HttpGet]
        [Authorize]
        [Route("transactions/summary")]
        public ActionResult GetSummary()
        {
            try
            {
                var result = transactionSummaryHandler.Get();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
