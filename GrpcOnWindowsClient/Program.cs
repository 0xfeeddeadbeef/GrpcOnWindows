using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcOnWindows;

static class Program
{
    static async Task Main(string[] args)
    {
        Greeter.GreeterClient client = new(
            GrpcChannel.ForAddress(
                new Uri("https://localhost:44364/"),
                new GrpcChannelOptions
                {
                    MaxRetryAttempts      = 2,
                    DisposeHttpClient     = true,
                    MaxReceiveMessageSize = 131072,
                    MaxSendMessageSize    = 131072,
                }));

        using var stream = client.SayHelloStream();

        var requestStream = stream.RequestStream;
        var responseStream = stream.ResponseStream;

        foreach (var n in Enumerable.Range(1, 10))
        {
            await requestStream.WriteAsync(new HelloRequest { Name = $"Person{n}" }).ConfigureAwait(false);

            Console.WriteLine("Sent request.");

            await responseStream.MoveNext<HelloReply>().ConfigureAwait(false);
            var reply = responseStream.Current;

            Console.WriteLine("Reply: '{0}'", reply.Message);
        }

        await requestStream.CompleteAsync().ConfigureAwait(false);

        Console.WriteLine("Closed the stream.");
    }
}
