using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElevatorServer.ElevatorService;

public class ElevatorRunnerBackgroundService : IHostedService, IDisposable
{
    private Task? _executingTask;
    private readonly IElevatorComService _elevatorComService;
    private ElevatorsOptions _elevatorOptions;
    private readonly ILogger<ElevatorRunnerBackgroundService> _logger;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private readonly IDictionary<int, Elevator> _elevators = new Dictionary<int, Elevator>();

    public ElevatorRunnerBackgroundService(IOptions<ElevatorsOptions> elevatorOptions, IElevatorComService elevatorComService, ILogger<ElevatorRunnerBackgroundService> logger)
    {
        _elevatorOptions = elevatorOptions.Value;
        _elevatorComService = elevatorComService;
        _logger = logger;
    }

    public async Task RunElevator(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ElevatorCommand? command;
            do
            {
                command = await _elevatorComService.GetNextCommand();
                if (command != null)
                {
                    _elevators[command.ElevatorId].IssueCommand(command.Floor);
                }

            } while (command != null);

            foreach (var elevator in _elevators.Values)
            {
                var oldState = elevator.ElevatorState;
                elevator.IterateElevator();
                await _elevatorComService.UpdateElevatorState(elevator.ToDTO());
                if (oldState != elevator.ElevatorState)
                {
                    _logger.LogInformation($"{DateTime.Now} Elevator [{elevator.ElevatorId}]: {oldState} => {elevator.ElevatorState}");
                }

            }

            await Task.Delay(_elevatorOptions.UpdateRateMs);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(ElevatorRunnerBackgroundService)} has started");

        AddElevators();

        _executingTask = RunElevator(_stoppingCts.Token);

        if (_executingTask.IsCompleted)
        {
            return _executingTask;
        }

        return Task.CompletedTask;
    }

    private void AddElevators()
    {
        foreach (var id in Enumerable.Range(0, _elevatorOptions.NumberOfElevators))
        {
            var elevator = new Elevator(id, _elevatorOptions.Floors, maxSpeed: _elevatorOptions.ElevatorSpeed);
            _elevators.Add(id, elevator);
            _elevatorComService.ConnectElevator(elevator);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }
        try
        {
            _stoppingCts.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    public void Dispose()
    {
        _stoppingCts.Cancel();
    }
}

