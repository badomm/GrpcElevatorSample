
namespace ElevatorServer.ElevatorService
{
    public interface IElevatorBrain
    {
        Task AddFromFloorRequest(FromRequest request);
        Task AddToFloorRequest(ToRequest request);
    }
}