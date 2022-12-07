namespace Stas.GA;
internal class AreaTransition: EntComp {
    public AreaTransition(IntPtr address) : base(address) {
    }

    internal override void Tick(nint ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        WorldAreaId = ui.m.Read<ushort>(Address + 0x28);
        //WorldAreaDat AreaById = ui.curr_loaded_files.WorldAreas.GetAreaByAreaId(WorldAreaId);
        //WorldAreaDat WorldArea = ui.curr_loaded_files.WorldAreas.GetByAddress(ui.m.Read<long>(Address + 0x48));
        TransitionType = (AreaTransitionType)ui.m.Read<byte>(Address + 0x2A);
    }
    public int WorldAreaId { get; private set; }
    public WorldAreaDat AreaById { get; private set; }
    public WorldAreaDat WorldArea { get; private set; }
    public AreaTransitionType TransitionType { get; private set; }
}
public enum AreaTransitionType {
    Normal = 0,
    Local = 1,
    NormalToCorrupted = 2,
    CorruptedToNormal = 3,
    Labyrinth = 5
}
