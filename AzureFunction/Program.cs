using Infrastructure.BlobAccess;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Mail;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<SmtpClient>(provider => new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SenderEmail"), Environment.GetEnvironmentVariable("SenderEmailPassword")),
            EnableSsl = true,
        });

        services.AddScoped<IBlobStorage>(provider => new BlobStorage(
            Environment.GetEnvironmentVariable("BlobConnectionString")!,
            Environment.GetEnvironmentVariable("BlobContainerName")!));

        services.AddSingleton<IEmailService, EmailService>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();
    })
    .Build();

host.Run();
