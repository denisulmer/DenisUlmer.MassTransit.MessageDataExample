namespace DenisUlmer.MessageDataExample.Service.Controllers;

using DenisUlmer.MessageDataExample.Service.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private readonly IRequestClient<SampleRequest> _requestClient;

    public DemoController(IRequestClient<SampleRequest> requestClient)
    {
        _requestClient = requestClient;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAsync([FromQuery] bool throwController)
    {
        if (throwController)
        {
            // Just to demo that the filter will catch this exception correctly
            throw new InvalidOperationException("Thrown in controller");
        }

        Response<AcceptedResponse, RejectedResponse> response = await _requestClient.GetResponse<AcceptedResponse, RejectedResponse>(new
        {
            SubmissionId = InVar.Id,

            // This prop is wrong, so the message data is empty in the request.
            ThisIsTheWrongDataProperty = "ok"
        });

        return Ok(response.Is<AcceptedResponse>(out _) ? "Accepted" : "Rejected");
    }
}
