namespace ElevatorServer.ElevatorService;

public class ElevatorsOptions
{
    public const string ConfigSection = "Elevators";

    public int NumberOfElevators { get; set; }

    public int Floors { get; set; }

    public double ElevatorSpeed { get; set; }

    public int UpdateRateMs { get; set; }
}

