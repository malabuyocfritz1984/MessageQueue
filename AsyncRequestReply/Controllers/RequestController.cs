using MessageBus;
using Microsoft.AspNetCore.Mvc;

namespace AsyncRequestReply.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestClient<ProcessPayload, ProcessPayloadResult> _requestClient;

        public RequestController(IRequestClient<ProcessPayload, ProcessPayloadResult> requestClient)
        {
            _requestClient = requestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string payload, CancellationToken cancellationToken)
        {
            var message = new ProcessPayload(payload);
            var response = await _requestClient.Get(message, cancellationToken);
            return Ok(response);
        }
    }
}
