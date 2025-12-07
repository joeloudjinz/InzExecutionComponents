using Demo.DummyApp.Resources;
using InzExecutionEvent;
using InzExecutionEvent.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInzExecutionComponents(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

app.Services.UseInzExecutionComponents();

var executionComponentManager = app.Services.GetRequiredService<IExecutionComponentManager>();
await executionComponentManager.LaunchExecution(ExecutionPlanKeys.Test);

Console.WriteLine("DONE!");