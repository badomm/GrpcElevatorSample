namespace ElevatorServer.ElevatorService;

public class FromFloorRequest
{
    public int FloorNumber { get; set; }
    public ElevatorMoveDirection Direction { get; set; }
}

public class ToFloorRequest
{
    public int FloorNumber { get; set; }
    public int ElevatorId { get; set; }

    public bool EmergencyStop { get; set; }
}

