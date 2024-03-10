using Infrastructure.BlobAccess;
using Infrastructure.Common.SmtpClientWrapper;
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

        services.AddSingleton<ISmtpClientWrapper>(provider => new SmtpClientWrapper(
            port: 587,
            credential: new NetworkCredential(Environment.GetEnvironmentVariable("SenderEmail"), Environment.GetEnvironmentVariable("SenderEmailPassword")),
            enableSsl: true
            ));

        services.AddScoped<IBlobStorage>(provider => new BlobStorage(
            Environment.GetEnvironmentVariable("BlobConnectionString")!,
            Environment.GetEnvironmentVariable("BlobContainerName")!));

        services.AddSingleton<IEmailService, EmailService>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();
    })
    .Build();

host.Run();
