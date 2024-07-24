var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts=> {
    var postgresCatalogDbConnection = builder.Configuration.GetConnectionString("CatalogDatabase");
    opts.Connection(postgresCatalogDbConnection!);
}).UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is null)
            return;

        var problemDetails = new ProblemDetails
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.StackTrace
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, exception.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.MapGet("/", () => "Catalog.API is running");

app.Run();