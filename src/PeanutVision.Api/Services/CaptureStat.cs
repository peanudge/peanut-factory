namespace PeanutVision.Api.Services;

/// <summary>
/// 시간 단위로 집계된 이미지 취득 건수 통계.
/// HourUtc 는 항상 분/초/밀리초가 0으로 정규화된 UTC DateTime 이다.
/// </summary>
public sealed class CaptureStat
{
    /// <summary>통계가 집계된 시각 (UTC, 정시 기준). Primary key.</summary>
    public DateTime HourUtc { get; set; }

    /// <summary>해당 시간대에 저장된 이미지 건수.</summary>
    public int Count { get; set; }
}
