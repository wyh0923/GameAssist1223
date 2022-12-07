using System.Runtime.InteropServices;
namespace Stas.GA;

internal class Flask : EntComp {
    public Flask(IntPtr address) : base(address) { }
    internal override void Tick(IntPtr ptr, string from=null) {
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<FlaskOffsets>(Address);
        var etern = ui.m.Read<FlaskExtendOffsets>(data.flaskExtPtr);
        stats.Tick(data.LocalStatComponent);
        quality.Tick(data.QualityComponent);
        var inst_1 = 0;
        stats.stats.TryGetValue(GameStat.LocalFlaskRecoversInstantly, out inst_1);
        var inst_2 = 0;
        stats.stats.TryGetValue(GameStat.LocalFlaskRecoveryAmountPctToRecoverInstantly, out inst_2);
        IsInstantRecovery = inst_1 != 0 ||  inst_2 >= 100;
    }
   
    public Stats stats { get; private set; } = new Stats(IntPtr.Zero);
    public Quality quality { get; private set; } = new Quality(IntPtr.Zero);
    public bool IsInstantRecovery { get; private set; }
    public int LifeRecover {//dpb
        get {//need upd GameStat list fomr poe GGPC
            stats.stats.TryGetValue(GameStat.LocalFlaskLifeToRecover, out var num);
            //stats.stats.TryGetValue(GameStat.LocalFlaskLifeToRecoverPosPct, out var num2);
            //stats.stats.TryGetValue(GameStat.LocalFlaskAmountToRecoverPosPct, out var num3);
            //num2 = num2 * 0.01f + 1f;
            //double num4 = num3 * 0.009999999776482582 + 1.0;
            //double num5 = (double)this.QualityComponent_0.ItemQuality * 0.01 + 1.0;
            //return (int)(num4 * (num5 * (double)num * (double)num2) + 0.5);
            return num;
        }
    }
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct FlaskOffsets {
    [FieldOffset(0x28)]  public IntPtr flaskExtPtr;
    [FieldOffset(0x30)] public IntPtr QualityComponent;
    [FieldOffset(0x38)] public IntPtr LocalStatComponent;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct FlaskExtendOffsets {
    [FieldOffset(0x18)] public int int_18;
    [FieldOffset(0x28)] public IntPtr struct132_0;
    [FieldOffset(0x42)] public IntPtr struct132_1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct Struct132 {
    // Token: 0x040006A1 RID: 1697
    private long intptr_0;

    // Token: 0x040006A2 RID: 1698
    public StdVector nativeVector_0;
}

