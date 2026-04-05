using PeanutVision.Api.Services;

namespace PeanutVision.Api.Tests.Unit;

/// <summary>
/// <see cref="CaptureStatService"/> 단위 테스트.
/// 저장소는 인메모리 스텁으로 대체한다.
/// </summary>
public class CaptureStatServiceTests
{
    // ---------------------------------------------------------------------------
    // In-memory stub repository
    // ---------------------------------------------------------------------------

    private sealed class StubCaptureStatRepository : ICaptureStatRepository
    {
        private readonly Dictionary<DateTime, CaptureStat> _data = new();

        public List<DateTime> IncrementedAt { get; } = new();

        public Task IncrementAsync(DateTime capturedAtUtc)
        {
            var bucket = Truncate(capturedAtUtc);
            IncrementedAt.Add(capturedAtUtc);

            if (_data.TryGetValue(bucket, out var stat))
                stat.Count++;
            else
                _data[bucket] = new CaptureStat { HourUtc = bucket, Count = 1 };

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<CaptureStat>> GetRangeAsync(DateTime fromUtc, DateTime toUtc)
        {
            var from = Truncate(fromUtc);
            var to   = Truncate(toUtc);
            IReadOnlyList<CaptureStat> result = _data.Values
                .Where(s => s.HourUtc >= from && s.HourUtc <= to)
                .OrderBy(s => s.HourUtc)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<CaptureStat>> GetRecentHoursAsync(int hours = 24)
        {
            var now  = DateTime.UtcNow;
            var from = Truncate(now).AddHours(-(hours - 1));
            var to   = Truncate(now);
            return GetRangeAsync(from, to);
        }

        public Task<int> GetTodayCountAsync()
        {
            var todayMidnight = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
                                            DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            var total = _data.Values
                .Where(s => s.HourUtc >= todayMidnight)
                .Sum(s => s.Count);
            return Task.FromResult(total);
        }

        private static DateTime Truncate(DateTime dt)
            => new(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
    }

    // ---------------------------------------------------------------------------
    // RecordCaptureAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task RecordCaptureAsync_delegates_to_repository_IncrementAsync()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var ts      = new DateTime(2026, 4, 5, 13, 42, 7, DateTimeKind.Utc);

        await service.RecordCaptureAsync(ts);

        Assert.Single(stub.IncrementedAt);
        Assert.Equal(ts, stub.IncrementedAt[0]);
    }

    [Fact]
    public async Task RecordCaptureAsync_multiple_times_accumulates_count_in_same_bucket()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var hour    = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);

        await service.RecordCaptureAsync(hour.AddMinutes(5));
        await service.RecordCaptureAsync(hour.AddMinutes(30));
        await service.RecordCaptureAsync(hour.AddMinutes(59));

        Assert.Equal(3, stub.IncrementedAt.Count);

        var stats = await service.GetRangeAsync(hour, hour);
        Assert.Single(stats);
        Assert.Equal(3, stats[0].Count);
    }

    [Fact]
    public async Task RecordCaptureAsync_different_hours_create_separate_buckets()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var base_   = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);

        await service.RecordCaptureAsync(base_.AddMinutes(10));   // 10:xx bucket
        await service.RecordCaptureAsync(base_.AddHours(1).AddMinutes(5)); // 11:xx bucket

        var stats = await service.GetRangeAsync(base_, base_.AddHours(1));
        Assert.Equal(2, stats.Count);
        Assert.Equal(1, stats[0].Count);
        Assert.Equal(1, stats[1].Count);
    }

    // ---------------------------------------------------------------------------
    // GetRecentHoursAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task GetRecentHoursAsync_delegates_to_repository()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var now     = DateTime.UtcNow;

        await service.RecordCaptureAsync(now);

        var result = await service.GetRecentHoursAsync(24);
        Assert.Single(result);
    }

    // ---------------------------------------------------------------------------
    // GetRangeAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task GetRangeAsync_returns_only_stats_in_range()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var h10     = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);
        var h12     = new DateTime(2026, 4, 5, 12, 0, 0, DateTimeKind.Utc);
        var h14     = new DateTime(2026, 4, 5, 14, 0, 0, DateTimeKind.Utc);

        await service.RecordCaptureAsync(h10);
        await service.RecordCaptureAsync(h12);
        await service.RecordCaptureAsync(h14);

        var result = await service.GetRangeAsync(h10.AddHours(1), h12.AddHours(1));

        Assert.Single(result);
        Assert.Equal(h12, result[0].HourUtc);
    }

    [Fact]
    public async Task GetRangeAsync_returns_empty_when_no_data()
    {
        var stub    = new StubCaptureStatRepository();
        var service = new CaptureStatService(stub);
        var from    = new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc);
        var to      = new DateTime(2026, 4, 5, 23, 0, 0, DateTimeKind.Utc);

        var result = await service.GetRangeAsync(from, to);

        Assert.Empty(result);
    }
}
