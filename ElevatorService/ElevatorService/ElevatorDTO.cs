namespace ElevatorServer
{
    public class ElevatorDTO : IElevator
    {
        public double CurrentPosition  { get; set; }

        public int ElevatorId { get; set; }

        public ElevatorState ElevatorState { get; set; }

        public ElevatorMoveDirection MoveDirection { get; set; }

        public int MaxFloor { get; set; }

        public double MaxSpeed { get; set; }

        public int[] Commands { get; set; } = new int[] { };
    }
}
