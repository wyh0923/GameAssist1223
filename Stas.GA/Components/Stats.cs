namespace Stas.GA;
/// <summary>
/// WIP need updame 
/// </summary>
internal class Stats : EntComp {
    public Stats(IntPtr address) : base(address) {

    }
    int last_hash = 0;
    DateTime next_get = DateTime.Now;
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero || next_get > DateTime.Now)
            return;
        next_get = next_get.AddMilliseconds(100);
        var comp = ui.m.Read<StatsComponent>(Address);
        var sub = ui.m.Read<SubStatsComponent>(comp.SubStatsPtr);
        var curr_hash = sub.Stats.GetHashCode();
        if (curr_hash != last_hash) {
            last_hash = curr_hash;
            StatsCount = sub.Stats.TotalElements(8);
            var first = sub.Stats.First.ToInt64();
            var last = sub.Stats.Last.ToInt64();
            var end = sub.Stats.End.ToInt64();
            if (sub.Stats.TotalElements(1) <= 0) {
                stats.Clear();
                return;
            }

            var total_stats = last - first;
            var max_stats = end - first;
            if (total_stats > max_stats || first == 0 || max_stats > 9000) {
                ui.AddToLog(tName + ".ParseStats same init err", MessType.Error);
                stats.Clear();
                return;
            }
            var data = ui.m.ReadMemoryArray<byte>(new IntPtr(first), (int)total_stats);
            var capacity = max_stats / 8;
            if (capacity < 0) {
                ui.AddToLog(tName + ".ParseStats capacity<0", MessType.Error);
                stats.Clear();
                return;
            }
            if (data.Length < capacity)
                stats = new Dictionary<GameStat, int>((int)capacity);

            for (var i = 0; i < data.Length - 0x04; i += 8) {
                try {
                    var key = BitConverter.ToInt32(data, i);
                    var value = BitConverter.ToInt32(data, i + 0x04);
                    stats[(GameStat)key] = value;
                }
                catch (Exception e) {
                    ui.AddToLog(tName + ".ParseStats err: " + e.Message, MessType.Error);
                }
            }
        }

    }
    public Dictionary<GameStat, int> stats = new();
    public long StatsCount { get; private set; }
  
}
