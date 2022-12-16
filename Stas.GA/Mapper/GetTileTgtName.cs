using System.Collections.Concurrent;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;

namespace Stas.GA;
public partial class AreaInstance {        //
    #region per area data here - nyst be clearing after map reloaded:
    public ConcurrentDictionary<uint, StaticMapItem> static_items = new();
    ConcurrentDictionary<eTypes, ConcurrentDictionary<uint, Entity>> bad_etypes = new();
    public ConcurrentBag<Entity> exped_keys = new();
    public ConcurrentBag<Entity> exped_beams = new();
    Dictionary<uint, string> id_ifos = new(); //map loading cleare
    public Dictionary<string, MapItem> bad_map_items = new();
    public List<Entity> need_check = new();//not processed yet ent 

    #endregion
    #region debug stuff
    /// <summary>
    /// Debug frame item
    /// </summary>
    public aMapItem mi_debug;
    aMapItem frame_di;
    public string debug_info = "Mapper info";

    public Entity marked { get; private set; }
    /// <summary>
    /// debug ent ID(for editor)
    /// </summary>
    public int debug_id = 0;
    /// <summary>
    ///saved map items
    /// </summary> 
    #endregion
    miType[] portals = { miType.portal, miType.transit, miType.IncursionPortal };
    public List<StaticMapItem> transit_close {
        get {
            return static_items.Values.Where(i => i.ent.IsValid
                && portals.Any(p => i.m_type == p)
                && i.gdist_to_me < 300)
                .OrderBy(x => x.gpos.GetDistance(ui.me.gpos)).ToList();
        }
    }

    public void Set_GridCell_as_rout(V2 gp) {
        if (!ui.nav.b_ready)
            return;
        var gc = ui.nav.Get_gc_by_gp(gp);
        if (gc != null) {
            gc.Set_all_cell_rout();
        }
        else {
            ui.AddToLog("Make all rout err: gc not found");
        }
    }
    readonly string heistUsefullChestContains = "HeistChestSecondary";
    readonly string heistAllChestStarting = "Metadata/Chests/LeagueHeist";
    readonly Dictionary<uint, string> heistChestCache = new();
    private string HeistChestPathToIcon(string path) {
        var strToReplace = string.Join('/', this.heistAllChestStarting, this.heistUsefullChestContains);
        var truncatedPath = path
            .Replace(strToReplace, null, StringComparison.Ordinal)
            .Replace("Military", null, StringComparison.Ordinal)
            .Replace("Thug", null, StringComparison.Ordinal)
            .Replace("Science", null, StringComparison.Ordinal)
            .Replace("Robot", null, StringComparison.Ordinal);
        return $"Heist {truncatedPath}";
    }
    readonly Dictionary<uint, string> delveChestCache = new();
    readonly string delveChestStarting = "Metadata/Chests/DelveChests/";
    private string DelveChestPathToIcon(string path) {
        return path.Replace(this.delveChestStarting, null, StringComparison.Ordinal);
    }
    public ConcurrentBag<MapItem> map_items = new();
    public ConcurrentBag<Cell> triggers = new ConcurrentBag<Cell>();
    TileStructure[] tileData;
    /// <summary>
    /// we need nav.grid_sells ready for link tgp name to it
    /// </summary>
    public void GetTileTgtName() {
        var sw = new SW("GetTileByFname");
        var td = ui.curr_map.terr_meta_data;
        Debug.Assert(ui.nav.b_ready && td.TileDetailsPtr.First != default);//

        var skip = 0;
        var ok = 0;
        sw.Restart();
        //TgtTilesLocations = GetTgtFileData();
        tileData = ui.m.ReadStdVector<TileStructure>(td.TileDetailsPtr);

        var err_ptr = 0;
#if DEBUG
        for (int i = 0; i < tileData.Length; i++) {//for step-by-step debugging use here
            Run(i);
        }
#else
        Parallel.For(0, tileData.Length, i => {
            Run(i);
        });
#endif
        void Run(int i) {
            if (td.BytesPerRow == 0) { //it hapened if poe was closed and memory wrong
                return;
            }
            var x = (int)(i / td.TotalTiles.X) * TileStructure.TileToGridConversion;
            var y = (int)(i % td.TotalTiles.X) * TileStructure.TileToGridConversion;
            var gc = ui.nav.grid_cells.FirstOrDefault(g => g.min.X == y && g.min.Y == x);
            if (gc != null) {
                var ptr = tileData[i].TgtFilePtr;
                if (ptr == IntPtr.Zero) {
                    err_ptr += 1;
                    ui.AddToLog("TgtFilePtr ==zero");
                    skip += 1;
                }
                else {
                    var file = ui.m.Read<TgtFileStruct>(ptr);
                    var path = ui.m.ReadStdWString(file.TgtPath);
                    gc.path = path;
                    #region Dont use jet
                    //var tgt = ui.m.Read<TgtDetailStruct>(file.TgtDetailPtr).name;
                    //var key = ui.m.ReadStdWString(tgt);
                    //gc.tile_key = key;
                    #endregion
                    ok += 1;
                }
            }
            else
                skip += 1;
        }
        ui.AddToLog("Title create time=[" + sw.ElapsedTostring() + "] ok/err/skipp/totale=[" +
           err_ptr + "/" + ok + "/" + skip + "/" + ui.nav.grid_cells.Count + "]", MessType.Warning);
        //sw.Print();
    }
}
public class Remnant {
    public Dictionary<string, int> positive = new();
    public Dictionary<string, int> negative = new();

}
