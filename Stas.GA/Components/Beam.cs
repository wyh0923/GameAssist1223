using V3 = System.Numerics.Vector3;
namespace Stas.GA;
internal class Beam : EntComp {
    public Beam(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        BeamStart = ui.m.Read<V3>(Address + 0x50);//beam start is actually the entity world pos
        BeamEnd = ui.m.Read<V3>(Address + 0x5C);
        Unknown1 = ui.m.Read<int>(Address + 0x40);//looks like 2 bools
        Unknown2 = ui.m.Read<int>(Address + 0x44);
    }

    internal V3 BeamStart { get; private set; }
    internal V3 BeamEnd { get; private set; }
    internal int Unknown1 { get; private set; }
    internal int Unknown2 { get; private set; }
}
