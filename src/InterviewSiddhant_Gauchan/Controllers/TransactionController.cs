using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrulayerApiTest.Handlers.Query;

namespace TrulayerApiTest.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TransactionController : ControllerBase
    {
        
        private readonly IMediator mediator;

        public TransactionController(IMediator mediator)
        {
            
            this.mediator = mediator;
        }
        
        [HttpGet]
        [Route("transactions")]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            try
            {
                var result = await mediator.Send(new GetTransactionQuery());                    
                return Ok(result.Response);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
                
        [HttpGet]
        [Authorize]
        [Route("transactions/summary")]
        public async Task<ActionResult> GetSummary()
        {
            try
            {
                var result = await mediator.Send(new GetTransactionSummaryQuery());
                return Ok(result.Response);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
