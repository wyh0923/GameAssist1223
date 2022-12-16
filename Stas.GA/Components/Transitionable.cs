namespace Stas.GA;
internal class Transitionable : EntComp {
    public Transitionable(IntPtr ptr) : base(ptr) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        Flag1 = ui.m.Read<byte>(Address + 0x120);
        Flag2 = ui.m.Read<byte>(Address + 0x124);
    }
    public byte Flag1 { get; private set; }
    public byte Flag2 { get; private set; }

    protected override void Clear() {

    }
}