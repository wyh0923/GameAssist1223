namespace Stas.GA;
using System;

/// <summary>
///     The <see cref="NPC" /> component in the entity.
/// </summary>
public class NPC : EntComp {
    public NPC(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        HasIconOverhead = ui.m.Read<long>(Address + 0x48) != 0;
        IsIgnoreHidden = ui.m.Read<byte>(Address + 0x20) == 1;
        IsMinMapLabelVisible = ui.m.Read<byte>(Address + 0x21) == 1;
    }
    DateTime next_upd = DateTime.Now;

    public bool HasIconOverhead { get; private set; }
    public bool IsIgnoreHidden { get; private set; }
    public bool IsMinMapLabelVisible { get; private set; }
}
