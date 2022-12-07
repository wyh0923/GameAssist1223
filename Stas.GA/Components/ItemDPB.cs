namespace Stas.GA;
public class ItemDPB : RemoteObjectBase {
    int _localId;

    public ItemDPB(IntPtr ptr, bool hasInventoryLocation,  Vector2i locationBottomRight, Vector2i locationTopLeft):base(ptr) {
        this.LocationBottomRight = locationBottomRight;
        this.LocationTopLeft = locationTopLeft;
        this.HasInventoryLocation = hasInventoryLocation;
        this._localId =ui.m.Read<int>(Address + 2920L); //7733358
    }

    public bool IsValid {
        get {
            long addr = Address + 8L;
            var curr_val = ui.m.Read<int>(addr); //-2117284256
            return curr_val == 6619213;
        }
    }
    public string Metadata {
        get {
           //var res= ui.m.ReadStringU(ui.m.Read<long>(Address + 8L));
            return null;
        }
    }

    public Vector2i LocationBottomRight { get; }
    public Vector2i LocationTopLeft { get; }
    public bool HasInventoryLocation { get; }

   

    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        ui.AddToLog(tName + ".Tick need implement", MessType.Critical);

    }
    protected override void CleanUpData() {
        ui.AddToLog(tName + ".CleanUpData need implement", MessType.Critical);

    }
}