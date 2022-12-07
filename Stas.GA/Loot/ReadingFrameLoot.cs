#region using
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using Stas.POE.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;

#endregion

namespace Stas.GameAssist {
    public partial class Looter {
        string morphPath = "Metadata/MiscellaneousObjects/Metamorphosis/MetamorphosisMonsterMarker";
        List<uint> need_delete = new List<uint>();
        void ReadingFrameLoot() {
            var la = ui.gui.ItemsOnGroundLabels;
            if (la == null) {
                ui.AddToLog("Looter Err: ItemsOnGroundLabels==null", MessType.Error);
                Thread.Sleep(500);
                return;
            }
            need_delete.Clear();
            frame_keys.Clear();
            //var all_sorted = la.OrderBy(e => e.ItemOnGround?.gdist_to_me).ToArray();
            var close = la.Where(e => e.Label != null && e.Label.IsValid && e.ItemOnGround != null
            && e.ItemOnGround.IsValid && e.ItemOnGround.gdist_to_me < loot_dist)
                .OrderBy(e => e.ItemOnGround?.gdist_to_me).ToList();
          
            foreach (var l in la) { //9896.739, 8516.304, -211.15015
                var e = l.ItemOnGround;
                var cpa = l.CanPickUp;
                if (e == null || !e.IsValid || e.Pos == V3.Zero || bad_labels.Contains(l.Address) )
                    continue; //|| Math.Abs(ui.me.Pos.Z - e.Pos.Z) > 50
              
                if (!e.GetComp<WorldItem>(out var worldItem))
                    continue;
                //e.GetComp<Targetable>(out var trg);
                var item_ent = worldItem.ItemEntity;
                //todo: stop here for debug same
                if (item_ent.Address_hex == "item_ent") { 
                }
                if (item_ent.GetComp<ArchnemesisMod>(out var anm)) {
                    var bc = l.Label.BackgroundColor;
                    if(bc.G!=255 ||bc.B!=255 ) {
                        bad_labels.Add(l.Address);
                        continue;
                    }
                    //ui.test_elem = l.Label;
                }
                if (item_ent?.Path == null || item_ent?.Path.Length < 1)
                    continue;
                try {
                    var key = e.GetKey;
                    if (loot_items.ContainsKey(key)) {
                        var bit = ui.gc.Files.BaseItemTypes.Translate(item_ent.Path);
                        var old = loot_items[key].loot.BaseName == bit.BaseName
                          && loot_items[key].loot.ClassName == bit.ClassName;
                        if (old) {
                            loot_items[key].Update(l);
                            frame_keys.Add(key);
                            continue;
                        } else {
                            ui.AddToLog("wrong loot ent.key", MessType.Error);
                        }
                    }
                    var loot = new Loot(l, item_ent);
                    var bna = loot.BaseName.Split(" ");
                    var last_base_name = bna[bna.Length - 1];
                    var bad_rarity = loot.Rarity < ItemRarity.Rare || loot.IsIdentified;
                    var b_chaos_Set = chaos_set.Contains(loot.ClassName) && !bad_rarity;
                    var b_need = item_i_need.Contains(loot.ClassName) ;
                    var b_6s = false;
                    if(loot.Sockets == 6){
                        if ((loot.Height == 4 && sett.b_6s_big) || (loot.Height == 3 && sett.b_6s_small))
                            b_6s = true;
                    }
                    if (!b_need  && !b_6s && !b_chaos_Set)
                        continue;
                
                    var mi = MakeLootMapItem(loot);
                    if (mi != null) {
                        loot_items[mi.key] = mi;
                        frame_keys.Add(key);
                    }
                } catch (Exception ex) {
                    ui.AddToLog("AddLoot Err: " + ex.Message);
                    continue;
                }
            }
            //checking for the validity of the loot static cash in loot_dist radius
            //by comparing with the one received in the last frame
            //necessary if we collecting a loot manually
            foreach (var l in loot_items.Where(li => 
                        li.Value.gpos.GetDistance(ui.me.gpos) < loot_dist)) {
                if (!frame_keys.Contains(l.Key))
                    need_delete.Add(l.Key);
            }
            foreach (var l in need_delete)
                loot_items.TryRemove(l, out _);
            debug_info = "loot=[" + la.Count + "/" + close.Count + "/" + loot_items.Count + "]"; 
            void DebugFlasks() {
                var flask = la.FirstOrDefault(l => l.ToString() == "Eternal Mana Flask");
                ui.AddToLog("count=[" + la.Count + "] pos=[" + flask?.Label.Position.ToString() + "]");
            }
        }

       
    }

   
}
