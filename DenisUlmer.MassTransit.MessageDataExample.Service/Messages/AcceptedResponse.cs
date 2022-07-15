namespace DenisUlmer.MessageDataExample.Service.Messages;

using MassTransit;

public record AcceptedResponse
{
    public Guid SubmissionId { get; init; }
}