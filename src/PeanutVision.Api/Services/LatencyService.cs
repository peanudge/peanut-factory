namespace PeanutVision.Api.Services;

/// <summary>
/// Composes <see cref="ILatencyRepository"/> (storage) and <see cref="LatencyAnalyzer"/>
/// (computation) behind the <see cref="ILatencyService"/> abstraction.
/// </summary>
public sealed class LatencyService : ILatencyService
{
    private readonly ILatencyRepository _repository;

    public LatencyService(ILatencyRepository repository)
        => _repository = repository;

    public void Record(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId)
        => _repository.Add(triggerSentAt, frameReceivedAt, frameIndex, profileId);

    public IReadOnlyList<LatencyRecord> GetRecent(int max = 200)
        => _repository.GetRecent(max);

    public LatencyStats? GetStats()
        => LatencyAnalyzer.Compute(_repository.GetRecent(int.MaxValue));

    public void Clear()
        => _repository.Clear();
}
