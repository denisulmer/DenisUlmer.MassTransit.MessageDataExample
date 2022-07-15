using Azure.Storage.Blobs;
using DenisUlmer.MessageDataExample.Service;
using DenisUlmer.MessageDataExample.Service.Saga;
using MassTransit;
using MassTransit.MessageData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(x =>
{
    x.Filters.Add<GlobalExceptionFilter>();
})
.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<SampleStateMachine, SampleState>().InMemoryRepository();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.UseMessageData(new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")).CreateMessageDataRepository("masstransit-message-data"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseAuthorization();

app.MapControllers();

app.Run();
