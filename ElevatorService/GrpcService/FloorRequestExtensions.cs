using ElevatorGrpc;
using ElevatorServer.ElevatorService;

namespace ElevatorServer.GrpcService;

public static class FloorRequestExtensions
{
    public static FromRequest ConvertToFromRequest(this FromFloorRequest input) => new FromRequest()
    {
        FloorNumber = input.FloorNumber,
        Direction = (ElevatorMoveDirection)input.Direction
    };

    public static ToRequest ConvertToToRequest(this ToFloorRequest input) => new ToRequest()
    {
        FloorNumber = input.FloorNumber,
        ElevatorId = input.ElevatorId,
        EmergencyStop = input.EmergencyStop
    };
}
