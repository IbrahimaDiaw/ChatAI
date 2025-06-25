using ChatBackend.Hubs;
using ChatBackend.Services.AI;
using ChatBackend.Services.Chat;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
                "https://localhost:7176",
                "https://localhost:44323/"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Semantic Kernel
builder.Services.AddScoped<Kernel>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var kernelBuilder = Kernel.CreateBuilder();

    var endpoint = configuration["AzureOpenAI:Endpoint"];
    var apiKey = configuration["AzureOpenAI:ApiKey"];
    var deploymentName = configuration["AzureOpenAI:DeploymentName"];

    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: apiKey
    );

    return kernelBuilder.Build();
});

builder.Services.AddSingleton<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IAIService, AIService>();

builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors();
app.MapHub<ChatHub>("/chathub");

app.MapGet("/", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();
