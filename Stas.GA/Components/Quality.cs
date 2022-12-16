using System.Runtime.InteropServices;

namespace Stas.GA;
internal class Quality : EntComp {
    public Quality(IntPtr ptr) : base(ptr) {
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<QualityOffsets>(Address);
         ItemQuality  = ui.m.Read<int>(data.itemQuality);
         MaxQuality = ui.m.Read<int>(data.qualityExt_ptr.maxQuality);
    }
    
    public int ItemQuality { get; private set; }
    public int MaxQuality { get; private set; }
    protected override void Clear() {
        base.Clear();
    }

}
[StructLayout(LayoutKind.Explicit, Pack = 1)]
internal struct QualityOffsets {
    [FieldOffset(0x0)] public ComponentHeader header;
    [FieldOffset(0x10)]public QualityExtOffsets qualityExt_ptr;
    [FieldOffset(0x18)] public int itemQuality; //18
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
internal struct QualityExtOffsets {
    [FieldOffset(0x0)] public IntPtr ptr_0;
    [FieldOffset(0x08)] public IntPtr ptr_8;
    [FieldOffset(0x10)] public int maxQuality; //10
}

