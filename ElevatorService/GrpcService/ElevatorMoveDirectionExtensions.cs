using ElevatorGrpc;
using ElevatorServer;

public static class ElevatorMoveDirectionExtensions
{
    static public Direction ToElevatorMoveDirection(this ElevatorMoveDirection dir) => dir switch
    {
        ElevatorMoveDirection.UP => Direction.Up,
        ElevatorMoveDirection.DOWN => Direction.Down,
        ElevatorMoveDirection.NONE => Direction.Standby,
        _ => throw new NotImplementedException()
    };
}