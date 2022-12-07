using System.Runtime.InteropServices;
namespace Stas.GA;
//ExileAPI need combine with GA, dpb
//not used at atall jet
internal class ServerInventory : RemoteObjectBase {
    public ServerInventory(IntPtr ptr) : base(ptr) {
    }
    protected override void UpdateData() {
        var data = ui.m.Read<ServerInventoryOffsets>(Address);
        InventType = (InventoryTypeE)data.InventType;
        InventSlot = (InventoryName)data.InventSlot;
        Columns = data.Columns;
        Rows = data.Rows;
        IsRequested = data.IsRequested == 1;
        CountItems = data.CountItems;
        TotalItemsCounts = data.TotalItemsCount;
        ServerRequestCounter = data.ServerRequestCounter;
        InventorySlotItems = ReadHashMap(data.InventorySlotItemsPtr, hashReadLimit).Values.ToList();
        Hash = data.Hash;
    }
    internal override void Tick(string from) {
        if (Address == IntPtr.Zero)
            return;
        UpdateData();
    }
    private readonly int hashReadLimit = 1500;
    public Dictionary<int, InventSlotItem> ReadHashMap(long pointer, int limitMax) {
        var result = new Dictionary<int, InventSlotItem>();

        var stack = new Stack<HashNode>();
        var startNode = GetObject<HashNode>(pointer);
        var item = startNode.Root;
        stack.Push(item);

        while (stack.Count != 0) {
            var node = stack.Pop();

            if (!node.IsNull)
                result[node.Key] = node.Value1;

            var prev = node.Previous;

            if (!prev.IsNull)
                stack.Push(prev);

            var next = node.Next;

            if (!next.IsNull)
                stack.Push(next);

            if (limitMax-- < 0) {
                DebugWindow.LogError($"Fixed possible memory leak (ServerInventory.ReadHashMap @ {Address:X})");
                break;
            }
        }

        return result;
    }
    public class HashNode : RemoteMemoryObject {
        private readonly FrameCache<NativeHashNode> frameCache;

        public HashNode() {
            frameCache = new FrameCache<NativeHashNode>(() => M.Read<NativeHashNode>(Address));
        }

        private NativeHashNode NativeHashNode => frameCache.Value;
        public HashNode Previous => GetObject<HashNode>(NativeHashNode.Previous);
        public HashNode Root => GetObject<HashNode>(NativeHashNode.Root);
        public HashNode Next => GetObject<HashNode>(NativeHashNode.Next);

        //public readonly byte Unknown;
        public bool IsNull => NativeHashNode.IsNull != 0;

        //private readonly byte byte_0;
        //private readonly byte byte_1;
        public int Key => NativeHashNode.Key;

        //public readonly int Useless;
        public InventSlotItem Value1 => GetObject<InventSlotItem>(NativeHashNode.Value1);

        //public readonly long Value2;
    }
    public InventoryTypeE InventType { get; private set; } //FRom
    public InventoryName InventSlot { get; private set; } //from GH
    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public bool IsRequested { get; private set; }
    public long CountItems { get; private set; }
    public int TotalItemsCounts { get; private set; }
    public int ServerRequestCounter { get; private set; }
    public IList<InventSlotItem> InventorySlotItems { get; private set; }
    public long Hash { get; private set; }
    public IList<Entity> Items => InventorySlotItems.Select(x => x.Item).ToList();

    protected override void CleanUpData() {

    }

}
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ServerInventoryOffsets {
    [FieldOffset(0x138)] public byte InventType;
    [FieldOffset(0x13C)] public byte InventSlot;
    [FieldOffset(0x140)] public byte IsRequested;
    [FieldOffset(0x144)] public int Columns;
    [FieldOffset(0x148)] public int Rows;
    [FieldOffset(0x168)] public long InventoryItemsPtr;
    [FieldOffset(0x180)] public long InventorySlotItemsPtr;
    [FieldOffset(0x188)] public long CountItems;
    [FieldOffset(0x188)] public int TotalItemsCount;
    [FieldOffset(0x1C8)] public int ServerRequestCounter;
    [FieldOffset(0x1E8)] public long Hash;
}
/// <summary>
/// Copied from GGPK -> InventoryType.dat file
/// Possible improvement -> read it from the in memory dad file
/// </summary>
public enum InventoryTypeE {
    MainInventory = 0x00,
    BodyArmour,
    Weapon,
    Offhand,
    Helm,
    Amulet,
    Ring,
    Gloves,
    Boots,
    Belt,
    Flask,
    Cursor,
    Map,
    PassiveJewels,
    AnimatedArmour,
    Crafting,
    Leaguestone,
    Unused,
    Currency,
    Offer,
    Divination,
    Essence,
    Fragment,
    MapStashInv,
    UniqueStashInv,
    CraftingSpreeCurrency,
    CraftingSpreeItem,
    DelveTab,
    AtlasWatchtower,
    ExpeditionLocker,
    DeliriumStashInv,
    BlightStashInv,
    Harvest2,
    MetamorphStashInv,
    HeistAllyEquipment,
    Trinket,
    HeistLocker,
    StashRegularTab,
    Unknown38,
    Unknown39,
    Unknown40,
    Unknown41,
    Unknown42,
    Unknown43,
    UtzaalUltimatum,
    NormalOrQuad
}