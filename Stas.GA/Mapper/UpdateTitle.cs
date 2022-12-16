using System.Diagnostics;
using System.IO;
using System.Numerics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public partial class AreaInstance : RemoteObjectBase {
    private Dictionary<string, List<V2>> GetTgtFileData() {
        var tileData = ui.m.ReadStdVector<TileStructure>(this.terr_meta_data.TileDetailsPtr);
        var ret = new Dictionary<string, List<V2>>();
        object mylock = new();
        var bad_ptr = 0;
        Parallel.For(0, tileData.Length,
            // happens on every thread, rather than every iteration.
            () => new Dictionary<string, List<V2>>(),
            // happens on every iteration.
            (tileNumber, _, localstate) => {
                var tile = tileData[tileNumber];
                if (tile.TgtFilePtr == IntPtr.Zero) {
                    bad_ptr += 1;
                    ui.AddToLog("tileData bad ptr...", MessType.Error);
                    return localstate;
                }
                var tgtFile = ui.m.Read<TgtFileStruct>(tile.TgtFilePtr);
                var tgtName = ui.m.ReadStdWString(tgtFile.TgtPath);
                if (string.IsNullOrEmpty(tgtName)) {
                    return localstate;
                }

                if (tile.RotationSelector % 2 == 0) {
                    tgtName += $"x:{tile.tileIdX}-y:{tile.tileIdY}";
                }
                else {
                    tgtName += $"x:{tile.tileIdY}-y:{tile.tileIdX}";
                }

                var loc = new Vector2 {
                    Y = (tileNumber / this.terr_meta_data.TotalTiles.X) * TileStructure.TileToGridConversion,
                    X = (tileNumber % this.terr_meta_data.TotalTiles.X) * TileStructure.TileToGridConversion
                };

                if (localstate.ContainsKey(tgtName)) {
                    localstate[tgtName].Add(loc);
                }
                else {
                    localstate[tgtName] = new() { loc };
                }

                return localstate;
            },
            finalresult => // happens on every thread, rather than every iteration.
            {
                lock (mylock) {
                    foreach (var kv in finalresult) {
                        if (!ret.ContainsKey(kv.Key)) {
                            ret[kv.Key] = new();
                        }

                        ret[kv.Key].AddRange(kv.Value);
                    }
                }
            });
        if (bad_ptr < 0) {
            ui.AddToLog("tileData reading err=[" + bad_ptr + "]", MessType.Critical);
        }
        return ret;
    }

    public void AddImportantTile(V2 gp) {
        var gc = ui.nav.Get_gc_by_gp(gp);
        if (gc != null && gc.fname != null) {
            if (!ui.quest.tiles.Contains(gc.fname)) {
                ui.quest.tiles.Add(gc.fname);
                Debug.Assert(Directory.Exists("Quests"));
                ui.quest.Save();
                ui.AddToLog("Added [" + gc.fname + "] to [" + ui.curr_map_name + "]");
            }
        }
        else { ui.AddToLog("AddImportantTile title dont have a map name", MessType.Error); }
    }
}
