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

        while (true)
        {
            Console.WriteLine("Enter Command, example: [U1], [D2], [4], [stop]:");
            var line = Console.ReadLine();
            switch (line)
            {
                case "D0":
                case "D1":
                case "D2":
                case "D3":
                    var floorDown = int.Parse(line.Substring(1));
                    client.RequestFromFloor(new FromFloorRequest() { Direction = Direction.Down, FloorNumber = floorDown });
                    break;
                case "U0":
                case "U1":
                case "U2":
                case "U3":
                    var floorUp = int.Parse(line.Substring(1));
                    client.RequestFromFloor(new FromFloorRequest() { Direction = Direction.Down, FloorNumber = floorUp });
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                    var floor = int.Parse(line);
                    client.RequestToFloor(new  ToFloorRequest() { ElevatorId = 0, FloorNumber = floor });
                    break;
                case "STOP":
                case "stop":
                    client.RequestToFloor(new ToFloorRequest() { ElevatorId = 0, FloorNumber = -1 });
                    break;
                default:
                    Console.WriteLine("Invalid Command");
                    break;
            }
        }

    }
}