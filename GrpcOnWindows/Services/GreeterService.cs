namespace GrpcOnWindows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;

    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello, {request.Name}!"
            });
        }

        public override async Task SayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            await foreach (var query in requestStream.ReadAllAsync())
            {
                _logger.LogInformation("Request read.");

                if (context.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Stream has been cancelled.");

                    break;
                }

                var reply = new HelloReply { Message = $"Hello, {query.Name}!" };

                await responseStream.WriteAsync(reply).ConfigureAwait(false);

                _logger.LogInformation("Response sent.");
            }
        }
    }
}
