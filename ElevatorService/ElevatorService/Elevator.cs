namespace ElevatorServer.ElevatorService;

public class Elevator : IElevator
{
    private const int updateFreqMs = 10;
    private const double floorDelta = 0.01;
    private const int doorOpenTimeMs = 1000;
    private const int NOFLOOR = -1;

    private int doorTimerMs = 0;
    private HashSet<int> _commands;
    private int _currentCommand = NOFLOOR;

    public Elevator(int id, int maxFloor, int startFloor = 0, double maxSpeed = 0.2)
    {
        ElevatorId = id;
        CurrentPosition = startFloor;
        MaxSpeed = maxSpeed;
        MaxFloor = maxFloor;
        ElevatorState = ElevatorState.STANDBY;
        MoveDirection = ElevatorMoveDirection.NONE;
        _commands = new HashSet<int>();
    }

    public int ElevatorId { get; private set; }

    public double CurrentPosition { get; private set; }

    public ElevatorState ElevatorState { get; private set; }

    public double MaxSpeed { get; private set; }

    public int MaxFloor { get; private set; }

    public int[] Commands { get => _commands.ToArray(); }

    public ElevatorMoveDirection MoveDirection { get; private set; }

    public bool IssueCommand(int floorNumber)
    {
        if (floorNumber < 0)
        {
            EmergencyStop();
        }

        else if (floorNumber < MaxFloor)
        {
            return _commands.Add(floorNumber);
        }

        return false;
    }

    public void IterateElevator()
    {
        var newState = executeCurrentStateAndGetNextState();

        ElevatorState = newState;
    }

    public void EmergencyStop()
    {
        _currentCommand = NOFLOOR;
        MoveDirection = ElevatorMoveDirection.NONE;
        ElevatorState = ElevatorState.STANDBY;
        _commands.Clear();
    }

    public ElevatorDTO ToDTO() => new ElevatorDTO()
    {
        CurrentPosition = CurrentPosition,
        ElevatorId = ElevatorId,
        ElevatorState = ElevatorState,
        MoveDirection = MoveDirection,
        MaxFloor = MaxFloor,
        MaxSpeed = MaxSpeed,
        Commands = _commands.ToArray()
    };

    private int GetNextCommand()
    {
        if (!_commands.Any())
        {
            return NOFLOOR;
        }

        return MoveDirection switch
        {
            ElevatorMoveDirection.NONE => _commands.OrderBy(command => Distance(command)).DefaultIfEmpty(NOFLOOR).Min(),

            ElevatorMoveDirection.UP => _commands.Where(command => command >= CurrentPosition)
                                                 .OrderBy(command => Distance(command))
                                                 .DefaultIfEmpty(NOFLOOR)
                                                 .Min(),

            ElevatorMoveDirection.DOWN => _commands.Where(command => command <= CurrentPosition)
                                                   .OrderBy(command => Distance(command))
                                                   .DefaultIfEmpty(NOFLOOR)
                                                   .Min(),
            _ => throw new NotImplementedException()
        };
    }

    private double Distance(int destination)
    {
        return Math.Abs(CurrentPosition - destination);
    }

    private ElevatorState executeCurrentStateAndGetNextState()
    {
        _currentCommand = GetNextCommand();

        MoveDirection = GetMoveDirection(_currentCommand);
        return ElevatorState switch
        {
            ElevatorState.STANDBY => ExecuteStandbyState(_currentCommand),
            ElevatorState.DOOROPEN => ExecuteDoorOpenState(_currentCommand),
            ElevatorState.MOVING => ExecuteMoving(_currentCommand),
            _ => throw new NotImplementedException()
        };
    }

    private bool IsStopCommand(int command)
    {
        return command < 0;
    }

    private bool IsAtFloor(int floorCommand)
    {
        var floorDiff = floorCommand - CurrentPosition;
        return Math.Abs(floorDiff) < floorDelta;
    }

    private int GetMoveMultiplier(int floorCommand)
    {
        if (IsStopCommand(floorCommand))
        {
            return 0;
        }

        return floorCommand > CurrentPosition ? 1 : -1;
    }

    private ElevatorMoveDirection GetMoveDirection(int floorCommand)
    {
        if (IsStopCommand(floorCommand))
        {
            return ElevatorMoveDirection.NONE;
        }

        return floorCommand > CurrentPosition ? ElevatorMoveDirection.UP : ElevatorMoveDirection.DOWN; ;
    }

    private ElevatorState ExecuteStandbyState(int floorCommand)
    {
        // Action
        MoveDirection = ElevatorMoveDirection.NONE;
        // Transistion
        if (IsAtFloor(floorCommand))
        {
            return ElevatorState.DOOROPEN;
        }

        if (floorCommand > 0)
        {
            return ElevatorState.MOVING;
        }

        return ElevatorState.STANDBY;
    }

    private ElevatorState ExecuteDoorOpenState(int floorCommand)
    {
        // Action
        doorTimerMs += updateFreqMs;

        if (doorTimerMs < doorOpenTimeMs)
        {
            return ElevatorState.DOOROPEN;
        }

        CompleteFloorCommand(floorCommand);
        ResetDoortimer();

        // Transistion
        if (IsAtFloor(floorCommand))
        {
            return ElevatorState.STANDBY;
        }

        if (floorCommand < 0)
        {
            return ElevatorState.STANDBY;
        }

        return ElevatorState.MOVING;
    }

    private void CompleteFloorCommand(int floorCommand)
    {
        _commands.Remove(floorCommand);
    }

    private void ResetDoortimer()
    {
        doorTimerMs = 0;
    }

    private ElevatorState ExecuteMoving(int floorCommand)
    {
        // Action
        var dir = GetMoveMultiplier(floorCommand);


        var newFloorPosition = CurrentPosition + dir * MaxSpeed * updateFreqMs / 1000;
        CurrentPosition = Math.Min(MaxFloor, newFloorPosition);

        // Transistion
        if (IsAtFloor(floorCommand))
        {
            return ElevatorState.DOOROPEN;
        }

        if (floorCommand < 0)
        {
            return ElevatorState.STANDBY;
        }

        return ElevatorState.MOVING;
    }
}
