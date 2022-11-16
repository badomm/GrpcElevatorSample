namespace ElevatorServer.ElevatorService;

public class FromRequest
{
    public int FloorNumber { get; set; }
    public ElevatorMoveDirection Direction { get; set; }
}

public class ToRequest
{
    public int FloorNumber { get; set; }
    public int ElevatorId { get; set; }

    public bool EmergencyStop { get; set; }
}

