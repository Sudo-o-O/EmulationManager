using System.Diagnostics;
using EmulationManager.Models;

namespace EmulationManager.LaunchStrategies;

public interface ILaunchStrategy
{
    LaunchMethod Method { get; }

    Task<Process> LaunchAsync(
        LaunchRequest request,
        CancellationToken cancellationToken = default);
}
