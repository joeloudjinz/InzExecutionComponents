using InzExecutionEvent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.UseInzExecutionEvent(builder.Configuration)
    .RegisterExecutionComponentsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.Services.InitializeInzExecutionComponents();

Console.WriteLine("DONE!");