namespace Stas.GA; 

public class Stack : EntComp {
    public Stack(IntPtr ptr) : base(ptr) {

    }
    public int Size { get; private set; }
    public CurrencyInfo Info { get; } = new CurrencyInfo(IntPtr.Zero);
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address==IntPtr.Zero)
            return;
        Size = ui.m.Read<int>(Address + 0x18);
        Info.Tick(ui.m.Read<IntPtr>(Address + 0x10));
    }
   
    protected override void CleanUpData() {
        Info.Tick(IntPtr.Zero);
        Size = 0;
    }
}
public class CurrencyInfo : RemoteObjectBase {
    public CurrencyInfo(IntPtr ptr) : base(ptr) {

    }
    public int MaxStackSize { get; private set; }
    internal override void Tick(IntPtr ptr, string from=null) {
        if (Address == IntPtr.Zero)
            return;
        MaxStackSize = ui.m.Read<int>(Address + 0x28);
    }
    protected override void CleanUpData() {
        MaxStackSize = 0;
    }
}
