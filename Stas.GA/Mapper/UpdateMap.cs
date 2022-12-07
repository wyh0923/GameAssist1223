using System.Diagnostics;
using System.Drawing;
namespace Stas.GA;
public partial class AreaInstance {
    Stopwatch sw = new Stopwatch();
    public IntPtr map_ptr;
    public float progress = 0f;
    public int rows;
    public int cols;
    byte[] terrainBytes;
    public int[,] bit_data { get; private set; }
    public bool b_added_col { get; private set; }
    int make_ticks = 0; //for debug time creating
    /// <summary>
    /// after map creating only
    /// </summary>
    public bool b_ready { get; private set; }
    
    public void UpdateMap() {
        ClearOldData();
        ui.curr_map.UpdateMapDate();
        var inst_ok = this.Address != default;
        var state_ok = ui.curr_state == GameStateTypes.InGameState;
        while (!state_ok || !inst_ok) {
            ui.AddToLog("UpdateMap w8.. right state", MessType.Warning);
            Thread.Sleep(30);
        }
        Debug.Assert(state_ok && inst_ok);
      
        sw.Restart();
        var gridHeightData = GridHeightData;
        var terrainBytes = GridWalkableData;
        var td = TerrainMetadata;
        cols = (int)td.TotalTiles.X * 23;
        rows = (int)td.TotalTiles.Y * 23;
        var bytesPerRow = td.BytesPerRow;
        Debug.Assert(bytesPerRow > 0);
        if ((cols & 1) > 0) {
            cols++;
            b_added_col = true;
        }
        else
            b_added_col = false;
        while (td.TileDetailsPtr.First == IntPtr.Zero) {
            Thread.Sleep(50);
            ui.AddToLog("UpdateMap... w8 TileDetailsPtr...", MessType.Warning);
        }
        tileData = ui.m.ReadStdVector<TileStructure>(td.TileDetailsPtr);
        bit_data = new int[cols, rows];
        var bmp = new Bitmap(bytesPerRow * 2, terrainBytes.Length / bytesPerRow);
        Parallel.For(0, gridHeightData.Length, y => {
            for (var x = 1; x < gridHeightData[y].Length - 1; x++) {
                var index = (y * bytesPerRow) + (x / 2); // (x / 2) => since there are 2 data points in 1 byte.
                var shift = x % 2 == 0 ? 0 : 4;
                var both = terrainBytes[index];
                var bit = both >> shift & 0xF;
                bit_data[x, y] = bit;
                if (bit == 0)
                    continue;
                //TODO temporary - mb need filling array[color, color] or mb array[byte[4], byte[4]]
                //https://swharden.com/csdv/system.drawing/array-to-image/
                lock (bmp) { //we need it coz using Parallel
                    bmp.SetPixel(x, y, GetColor(bit));
                }
            }
        });
        map_ptr = ui.GetPtrFromImageData(bmp);
        bmp.Dispose();
        b_ready = true;
        ui.AddToLog("Map create time=[" + sw.ElapsedTostring() + "]", MessType.Warning); //853
        //ui.nav.MakeGridSells();
    }
    public List<Entity> need_check = new();

    void ClearOldData() {
        //todo: debag this list befor map change
        need_check.Clear();
        b_ready = false;
        ui.elements.Clear();
        ui.w8ting_click_until.Clear();
        environmentPtr = default;
        environments.Clear();
        MonsterLevel = 0;
        AreaHash = 0;
        server_data.Tick(IntPtr.Zero);
        player.Tick(IntPtr.Zero);
        TerrainMetadata = default;
        GridHeightData = Array.Empty<float[]>();
        GridWalkableData = Array.Empty<byte>();
        TgtTilesLocations.Clear();

        blight_pamp = null;
        blight_beams.Clear();
        bad_etypes.Clear();
        id_ifos.Clear();
        id_ifos.Clear();
        bad_map_items.Clear();
        static_items.Clear();
        make_ticks = 0;
        exped_keys.Clear();
        exped_beams.Clear();
        ui.nav.b_ready = false;//for not draw old visited
        ui.nav.debug_res = null;//same oldes debug must be deleted
        ui.test?.spa?.Clear(); //debug data need only actuale
    }
    internal static  Color GetColor(int i) {
        Color res;
        switch (i) {
            case 0:
                res = Color.FromArgb(0, 0, 0, 0);
                break;
            case 1:
                res = Color.FromArgb(20, 255, 255, 255);
                break;
            case 2:
                res = Color.FromArgb(50, 255, 255, 255);
                break;
            case 3:
                res = Color.FromArgb(90, 255, 255, 255);
                break;
            case 4:
                res = Color.FromArgb(25, 255, 255, 255);
                break;
            case 5:
                res = Color.FromArgb(15, 255, 255, 255);
                break;
            default:
                throw new Exception(i.ToString());
        }
        return res;
    }
}
