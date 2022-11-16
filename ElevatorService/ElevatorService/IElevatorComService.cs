namespace ElevatorServer.ElevatorService
{
    public interface IElevatorComService
    {
        Task<bool> ConnectElevator(IElevator elevator);
        Task<bool> DisconnectElevator(int id);
        Task<List<int>> GetElevatorIds();
        Task<IElevator> GetElevatorState(IElevator elevator);
        Task<List<IElevator>> GetAllElevatorState();
        Task<bool> UpdateElevatorState(IElevator elevator);
        Task IssueCommand(ElevatorCommand command);
        Task<ElevatorCommand?> GetNextCommand();

    }
}