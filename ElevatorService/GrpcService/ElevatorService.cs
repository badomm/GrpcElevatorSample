namespace ElevatorServer.Services;

using ElevatorGrpc;
using ElevatorServer.ElevatorService;
using ElevatorServer.GrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class ElevatorService : ElevatorGrpc.ElevatorService.ElevatorServiceBase
{
    private readonly IElevatorComService _comService;
    private readonly ILogger _logger;
    private readonly IElevatorBrain _brain;

    public ElevatorService(IElevatorComService comService, IElevatorBrain brain, ILogger<ElevatorService> logger)
    {
        _comService = comService;
        _logger = logger;
        _brain = brain;
    }

    public override async Task GetStatus(Empty request, IServerStreamWriter<StatusReply> responseStream, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        _logger.LogInformation($"GetStatus - Connection id: {httpContext.Connection.Id}");

        try
        {
            while (true)
            {
                var allElevatorStates = await _comService.GetAllElevatorState();
                var elevatorStatuses = allElevatorStates.Select(state =>
                    {
                        var status = new ElevatorStatus
                        {
                            ElevatorId = state.ElevatorId,
                            CurrentDirection = state.MoveDirection.ToElevatorMoveDirection(),
                            CurrentFloor = state.CurrentPosition,
                            DoorOpen = state.ElevatorState == ElevatorState.DOOROPEN,
                        };

                        var orders = state.Commands.Select(destinationFloor =>
                            new Order() {
                                ETA = GetETA(state, destinationFloor),
                                FloorNumber = destinationFloor
                            });

                        status.CurrentOrders.AddRange(orders);

                        return status;
                    }).ToList();

                StatusReply statusReply = new StatusReply();
                statusReply.ElevatorStatuses.AddRange(elevatorStatuses);

                await responseStream.WriteAsync(statusReply);
                await Task.Delay(200);
            }
        }
        catch (IOException)
        {
            _logger.LogInformation($"Connection {httpContext.Connection.Id} was aborted.");
        }

        return;
    }

    public override async Task<Empty> RequestFromFloor(FromFloorRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"From Floor Request: {request.FloorNumber}, {request.Direction}");
        await _brain.AddFromFloorRequest(request.ConvertToFromRequest());

        return new Empty();
    }

    public override async Task<Empty> RequestToFloor(ToFloorRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"To Floor Request: {request.FloorNumber}, Id: {request.ElevatorId}");
        await _brain.AddToFloorRequest(request.ConvertToToRequest());

        return new Empty();
    }

    private static double GetETA(IElevator elevator, int destinationFLoor)
    { 
        var diff = Math.Abs(elevator.CurrentPosition - destinationFLoor);

        return diff / elevator.MaxSpeed;
    }
}

