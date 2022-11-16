
namespace ElevatorServer.ElevatorService
{
    public interface IElevatorBrain
    {
        Task AddFromFloorRequest(FromFloorRequest request);
        Task AddToFloorRequest(ToFloorRequest request);
    }
}