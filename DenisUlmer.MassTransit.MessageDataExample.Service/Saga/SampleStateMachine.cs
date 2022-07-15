namespace DenisUlmer.MessageDataExample.Service.Saga;

using DenisUlmer.MessageDataExample.Service.Messages;
using MassTransit;
using System;

public class SampleStateMachine : MassTransitStateMachine<SampleState>
{
    public SampleStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartRequestReceived, x => x.CorrelateById(x => x.Message.SubmissionId).SelectId(x => x.Message.SubmissionId));

        Initially(When(StartRequestReceived)
            .Activity(x => x.OfType<ValidationActivity>())
            .IfElse(x => x.Saga.ValidationPassed,
                then => then
                .Then(x => Console.WriteLine("Transition to accepted"))
                    .TransitionTo(Accepted)
                    .Activity(x => x.OfType<PersistDataActivity>())
                    .TransitionTo(LocallyPersisted)
                    .RespondAsync(context => context.Init<AcceptedResponse>(new { context.Message.SubmissionId })),
                @else => @else
                .Then(x => Console.WriteLine("Transition to rejected"))
                .TransitionTo(Rejected)
                .RespondAsync(context => context.Init<RejectedResponse>(new { context.Message.SubmissionId }))
            )
        );
    }

    public State Accepted { get; private set; }
    public State Rejected { get; private set; }
    public State LocallyPersisted { get; private set; }

    public Event<SampleRequest> StartRequestReceived { get; private set; }
}

public class SampleState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public bool ValidationPassed { get; set; }
}