namespace DenisUlmer.MessageDataExample.Service.Messages;

using MassTransit;

public record RejectedResponse
{
    public Guid SubmissionId { get; init; }
}