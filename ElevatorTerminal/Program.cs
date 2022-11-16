using ElevatorGrpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var channel = GrpcChannel.ForAddress("http://localhost:56592", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
        var client = new ElevatorService.ElevatorServiceClient(channel);
        
        Console.CursorVisible = false;
        Console.WriteLine($"Starting");

        using (var streaming = client.GetStatus(new Empty()))
        {
            var getStatusTask = Task.Run(async () =>
            {
                while (await streaming.ResponseStream.MoveNext())
                {
                    var statuses = streaming.ResponseStream.Current.ElevatorStatuses;
                    foreach (var status in statuses)
                    {
                        Console.WriteLine(status);
                    }
                }
            });

            while (true) { }
        };
    }
}