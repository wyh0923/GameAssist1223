namespace Stas.GA;

internal class Animated : EntComp {
    public Animated(IntPtr address) : base(address) {

    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        test.Tick(ui.m.Read<IntPtr>(Address + 0x1C0));
        BaseAnimatedObjectEntity.Tick(ui.m.Read<IntPtr>(Address + 0x1E8));
    }
    public Entity test { get; private set; } = new Entity();
    public Entity BaseAnimatedObjectEntity { get; private set; } = new Entity();

    public void finder() {
        for (var i =0; i < 2048; i++) {
            var be = new Entity(ui.m.Read<IntPtr>(Address+ i));
            if (be.IsValid) {
                var offs = i.ToString("X"); //1C0 1E8
            }
        }
    }

    protected override void CleanUpData() {
        BaseAnimatedObjectEntity.Tick(IntPtr.Zero);
    }
}