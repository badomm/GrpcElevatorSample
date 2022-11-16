using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ElevatorServer.ElevatorService;

public class ElevatorComService : IElevatorComService
{
    private readonly ConcurrentDictionary<int, IElevator> _elevators;
    private readonly ConcurrentQueue<ElevatorCommand> _commands;
    private readonly ILogger<ElevatorComService> _logger;

    public ElevatorComService(ILogger<ElevatorComService> logger)
    {
        _elevators = new ConcurrentDictionary<int, IElevator>();
        _commands = new ConcurrentQueue<ElevatorCommand>();
        _logger = logger;
    }

    public async Task<bool> ConnectElevator(IElevator elevator)
    {
        var success = await Task.Run(() => _elevators.TryAdd(elevator.ElevatorId, elevator));
        if (success)
        {
            _logger.LogInformation($"Elevator with id [{elevator.ElevatorId}] has connonected");
        }
        return success;
    }

    public async Task<bool> DisconnectElevator(int id) => await Task.Run(() => _elevators.TryRemove(id, out _));

    public async Task<List<int>> GetElevatorIds() => await Task.Run(() => _elevators.Keys.ToList());

    public async Task<bool> UpdateElevatorState(IElevator elevator)
    {
        if (_elevators.TryGetValue(elevator.ElevatorId, out IElevator oldElevator))
        {
            return await Task.Run(() => _elevators.TryUpdate(elevator.ElevatorId, elevator, oldElevator));
        }
        return false;
    }

    public async Task<IElevator> GetElevatorState(IElevator elevator)
    {
        IElevator outElevator = null;
        _elevators.TryGetValue(elevator.ElevatorId, out outElevator);
        return outElevator;
    }

    public async Task<List<IElevator>> GetAllElevatorState()
    {
        return await Task.Run(() => _elevators.Values.ToList());
    }

    public async Task IssueCommand(ElevatorCommand command)
    {
        await Task.Run(() => _commands.Enqueue(command));
    }

    public async Task<ElevatorCommand?> GetNextCommand()
    {
        ElevatorCommand? result = default;
        await Task.Run(() => _commands.TryDequeue(out result));
        return result;
    }


}

