namespace DenisUlmer.MessageDataExample.Service.Messages;

using MassTransit;

public record SampleRequest
{
    public Guid SubmissionId { get; init; }
    public MessageData<string> Data { get; init; }
}
