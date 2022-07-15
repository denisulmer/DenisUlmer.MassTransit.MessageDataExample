namespace DenisUlmer.MessageDataExample.Service.Saga;

using DenisUlmer.MessageDataExample.Service.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

public class ValidationActivity : IStateMachineActivity<SampleState, SampleRequest>
{
    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<SampleState, SampleRequest> context, IBehavior<SampleState, SampleRequest> next)
    {
        Console.WriteLine("... Validating ...");

        string data = await context.Message.Data.Value;

        context.Saga.ValidationPassed = data == "ok";

        Console.WriteLine($"... ValidationPassed = {context.Saga.ValidationPassed} ...");

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<SampleState, SampleRequest, TException> context, IBehavior<SampleState, SampleRequest> next) where TException : Exception
    {
        return next.Faulted(context: context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("validate");
    }
}
