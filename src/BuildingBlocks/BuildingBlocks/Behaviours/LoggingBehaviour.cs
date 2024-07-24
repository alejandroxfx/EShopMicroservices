using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse>
        (ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
                    typeof(TRequest).Name, typeof(TResponse).Name, request);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next();

            timer.Stop();

            if (timer.Elapsed.Seconds > 3)
                logger.LogWarning("[PERFORMANCE] The request {Request} took {time}",
                    typeof(TRequest).Name, timer.Elapsed.Seconds);

            logger.LogInformation("[END] Handle request={Request} with Response={Response}",
                    typeof(TRequest).Name, typeof(TResponse).Name);

            return response;
        }
    }
}
