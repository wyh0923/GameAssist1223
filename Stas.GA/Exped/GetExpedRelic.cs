#region using
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
#endregion
namespace Stas.GA;
public partial class AreaInstance {
    public MapItem GetExpedRelic(Entity e) {
        var sett = ui.exped_sett;
        if (e.GetComp<ObjectMagicProperties>(out var omp)) {
            var mods = omp.Mods;
            Debug.Assert(mods != null);
            var remn = new Remnant();
            foreach (var m in mods) {
                if (m == "MonsterTotemAuraEnemyLifeDegen1") {//"BlightLightningType" 
                }
                else {
                    //Debug.Assert(m.StartsWith(ExpedSett.prefix));
                    foreach (var kv in sett.mods) {
                        if (m.Contains(kv.Key)) {
                            if (kv.Value > 0) {
                                remn.positive[kv.Key] = kv.Value;
                            }
                            else {
                                remn.negative[kv.Key] = kv.Value;
                            }
                            break;
                        }
                    }
                }

            }
            return asStaticMapItem(e, miType.ExpedRemnant, MapIconsIndex.Exped_remnant, "Table", IconPriority.Critical, remn);

        }
        else { //debug here
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.question_mark, "Error", IconPriority.Critical);
            //throw new NotImplementedException();
        }

        //if ((mods.Any(x => x.EndsWith("ImmunePhysicalDamage")) && sett.PhysImmune) ||
        //            (mods.Any(x => x.EndsWith("ImmuneFireDamage")) && sett.FireImmune) ||
        //            (mods.Any(x => x.EndsWith("ImmuneColdDamage")) && sett.ColdImmune) ||
        //            (mods.Any(x => x.EndsWith("ImmuneLightningDamage")) && sett.LightningImmune) ||
        //            (mods.Any(x => x.EndsWith("ImmuneChaosDamage")) && sett.ChaosImmune) ||
        //            (mods.Any(x => x.EndsWith("CannotBeCrit")) && sett.CritImmune) ||
        //            (mods.Any(x => x.EndsWith("ImmuneStatusAilments")) && sett.AilmentImmune) ||
        //            (mods.Any(x => x.EndsWith("CullingStrikeTwentyPercent")) && sett.Culling) ||
        //            (mods.Any(x => x.EndsWith("ElitesRegenerateLifeEveryFourSeconds")) && sett.Regen) ||
        //            (mods.Any(x => x.EndsWith("ExpeditionCorruptedItemsElite")) && sett.CorruptedItems) ||
        //            (mods.Any(x => x.EndsWith("AttackBlockSpellBlockMaxBlockChance")) && sett.BlockChance) ||
        //            (mods.Any(x => x.EndsWith("ResistancesAndMaxResistances")) && sett.MaxResistances) ||
        //            (mods.Any(x => x.EndsWith("CannotBeLeechedFrom")) && sett.NoLeech) ||
        //            (mods.Any(x => x.EndsWith("ImmuneToCurses")) && sett.NoCurse)) {
        //    return asStaticMapItem(e, MapItemType.ExpedRemnant, MapIconsIndex.ExpedWarning, "Warning");
        //}
    }
}

