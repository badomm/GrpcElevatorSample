namespace ElevatorServer;

public interface IElevator
{
    double CurrentPosition { get; }
    int ElevatorId { get; }
    ElevatorState ElevatorState { get; }

    ElevatorMoveDirection MoveDirection { get; }

    int MaxFloor { get; }
    double MaxSpeed { get; }

    int[] Commands { get; }
}