using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace Stas.GA {
    public partial class AreaInstance  {
        private Dictionary<string, List<Vector2>> GetTgtFileData() {
            var tileData = ui.m.ReadStdVector<TileStructure>(this.TerrainMetadata.TileDetailsPtr);
            var ret = new Dictionary<string, List<Vector2>>();
            object mylock = new();
            var bad_ptr = 0;
            Parallel.For( 0, tileData.Length,
                // happens on every thread, rather than every iteration.
                () => new Dictionary<string, List<Vector2>>(),
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
                        Y = (tileNumber / this.TerrainMetadata.TotalTiles.X) * TileStructure.TileToGridConversion,
                        X = (tileNumber % this.TerrainMetadata.TotalTiles.X) * TileStructure.TileToGridConversion
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

        public void AddImportantTile(System.Numerics.Vector2 gp) {
            //var gc = ui.nav.Get_gc_by_gp(gp);
            //if (gc != null && gc.fname != null) {
            //    var fname = @"C:\log\Quests\" + ui.curr_map_name + ".quest";
            //    if (!ui.tasker.quest.tiles.Contains(gc.fname)) {
            //        ui.tasker.quest.tiles.Add(gc.fname);
            //        ui.AddToLog("Added [" + gc.fname + "] to [" + ui.curr_map_name + "]");
            //        ui.tasker.quest.Save(fname);
            //    }
            //} else { ui.AddToLog("AddImportantTile title dont have a map name", MessType.Error); }

        }

    }
}
