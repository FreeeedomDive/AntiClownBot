using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AntiClown.Data.Api.Core.SettingsStoring.Telemetry;

public sealed class SettingsTelemetry
{
    public SettingsTelemetry(Meter meter)
    {
        reads = meter.CreateCounter<long>("anticlown.settings.reads", "{read}");
    }

    public void RecordRead(bool found)
    {
        var tags = new TagList
        {
            { "result", found ? Hit : Miss },
        };
        reads.Add(1, tags);
    }

    private const string Hit = "hit";
    private const string Miss = "miss";

    private readonly Counter<long> reads;
}
