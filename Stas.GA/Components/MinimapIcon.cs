using System.Runtime.InteropServices;
namespace Stas.GA;
/// <summary>
///     The <see cref="MinimapIcon" /> component in the entity.
/// </summary>
public class MinimapIcon : EntComp {
    public MinimapIcon(IntPtr address) : base(address) { 
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<MinimapIconOffsets>(this.Address);
        IsVisible = data.IsVisible == 1;
        IsHide = data.IsHide == 1;
        if (last_name == DateTime.MinValue) {
            var n_ptr = ui.m.Read<NativeStringU>(data.name_u);
            name = n_ptr.GetVaue();
            last_name = DateTime.Now;
        }
    }

    protected override void CleanUpData() {
        base.CleanUpData();
    }
    public bool IsVisible { get; private set; }
    public bool IsHide { get; private set; }
    public string name { get; private set; }
    DateTime last_name = DateTime.MinValue;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct MinimapIconOffsets {
    [FieldOffset(0x00)] public ComponentHeader header;
    [FieldOffset(0x20)] public IntPtr name_u;
    [FieldOffset(0x30)] public byte IsVisible;// wrong for same chest
    [FieldOffset(0x34)] public byte IsHide;
}