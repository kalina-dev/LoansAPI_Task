using LoansAPI.Database;
using LoansAPI.ExceptionHandler;
using LoansAPI.Extensions;
using LoansAPI.Repositories;
using LoansAPI.Services;
using Microsoft.OpenApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using LoansAPI.Data.Access.Repositories.Models;
using LoansAPI.Data.Access.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["allowedOrigins"]!).AllowAnyMethod()
        .AllowAnyHeader();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Loans API",
        Description = "This is loan operations Web Api v1",
        Contact = new OpenApiContact
        {
            Email = "kalexandrova@gmail.com",
            Name = "Kalina Aleksandrova",
            Url = new Uri("https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/overview")
        }
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<IDbDataGenerator, DbDataGenerator>();
builder.Services.AddSingleton<IDbFactory, DbFactory>();
builder.Services.AddSingleton<ICreditsRepository, CreditsRepository>();
builder.Services.AddSingleton<IInvoicesRepository, InvoicesRepository>();
builder.Services.AddSingleton<ICreditReportsService, CreditReportsService>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    await app.SeedDatabaseAsync();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error
    {
        Date = DateTime.UtcNow,
        ErrorMessage = exception.Message,
        StackTrace = exception.StackTrace
    };

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();
    repository.Create(error);

    await Results.BadRequest(new
    {
        type = "error",
        message = "an unexpected exception has occurred",
        status = 500
    }).ExecuteAsync(context);
}));

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/reports/credit/all", (ICreditReportsService creditReports) => 
        creditReports.GetAllCreditsAsync())
    .WithName("GetAllCreditsInformation");

app.MapGet("/reports/credit/statistics", (ICreditReportsService creditReports) => 
        creditReports.GetCreditStatisticsAsync())
    .WithName("GetCreditsStatistics");

app.Run();