using System.Drawing;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
internal class InventSlotItem : RemoteObjectBase {
    public InventSlotItem(IntPtr ptr) : base(ptr) {
    }
    protected override void UpdateData() {
        var data = ui.m.Read<ItemMinMaxLocation>(Address);

        Item.Address = ui.m.Read<IntPtr>(Address);
        var Location = ui.m.Read<ItemMinMaxLocation>(Address + 0x08);
        InventoryPosition = Location.InventoryPosition;
        PosX = ui.m.Read<int>(Address + 0x8);
        PosY = ui.m.Read<int>(Address + 0xc);
        SizeX = ui.m.Read<int>(Address + 0x10) - PosX;
        SizeY = ui.m.Read<int>(Address + 0x14) - PosY;

        var playerInventElement = ui.gui.player_inventory[InventoryIndex.PlayerInventory];
        var inventClientRect = playerInventElement.get_client_rectangle();
        var cellSize = inventClientRect.Width / 12;
        ClientRect = Location.GetItemRect(inventClientRect.X, inventClientRect.Y, cellSize);
    }
    internal override void Tick(string from) {
        if (Address == IntPtr.Zero)
            return;
        UpdateData();
    }
    public V2 InventoryPosition { get; private set; }


    public Entity Item { get; private set; } = new Entity(IntPtr.Zero)
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    RectangleF ClientRect; //for debug? not using at all jet
    protected override void CleanUpData() {
        base.CleanUpData();
    }

    private struct ItemMinMaxLocation {
        private int XMin { get; set; }
        private int YMin { get; set; }
        private int XMax { get; set; }
        private int YMax { get; set; }

        public RectangleF GetItemRect(float invStartX, float invStartY, float cellsize) {
            return new RectangleF(
                invStartX + cellsize * XMin,
                invStartY + cellsize * YMin,
                (XMax - XMin) * cellsize,
                (YMax - YMin) * cellsize);
        }

        public V2 InventoryPosition => new Vector2(XMin, YMin);

        public override string ToString() {
            return $"({XMin}, {YMin}, {XMax}, {YMax})";
        }
    }
}