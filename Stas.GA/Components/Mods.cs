namespace Stas.GA;
internal class Mods : EntComp {
    public Mods(nint address) : base(address) {
    }

    public bool Identified { get; private set; }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var mods_base = ui.m.Read<ModsComponentOffsets>(Address);
        var mods_detail = ui.m.Read<ModsComponentDetailsOffsets>(mods_base.ModsComponentDetailsKey);
        Identified = mods_base.Identified;
    }
  
    protected override void CleanUpData() {
        throw new NotImplementedException();
    }
}

