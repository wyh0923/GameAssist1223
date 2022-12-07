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

namespace Stas.GA {
    public partial class AreaInstance  {
        MapItem GetMonster(Entity e, MapItem mi) {
            e.GetComp<ObjectMagicProperties>(out var omp);
            if (e.eType == eTypes.Stage0FIT && omp.Rarity != Rarity.Unique) {
                return null;
            }
            e.GetComp<NPC>(out var _npc);
            e.GetComp<Life>(out var _life);
            var bad_npc = _npc != null && _npc.IsIgnoreHidden;
            if (_life == null || bad_npc) //|| !e.IsTargetable
                return null;
            if (e.IsFriendly) { //|| 
                                //if (ui.worker.b_use_balistas) { 
                                //}
                                //if (!e.Path.Contains("Summoned")) { 
                                //}
                frame_party.Add(e);
                if (ui.sett.b_show_frendly_mobs) {
                    mi.uv = sh.GetUV(MapIconsIndex.SmallWhiteCircle);
                    mi.priority = IconPriority.Low;
                    return mi;
                }
                return null;
            }
            else {
                if (e.rarity == Rarity.Unique && e.Path.Contains("Metadata/Monsters/Spirit/"))
                    mi.uv = sh.GetUV(MapIconsIndex.Spirit);

                e.Stats?.TryGetValue(GameStat.FrozenInTime, out var FrozenInTime);
                e.Stats?.TryGetValue(GameStat.MonsterHideMinimapIcon, out var MonsterHideMinimapIcon);
                e.GetComp<Buffs>(out var buffs);
                if (buffs != null && buffs.StatusEffects.ContainsKey("proximity_shield_aura")) {
                    //archnemesis_bubble_shield 
                    mi.uv = sh.GetUV(MapIconsIndex.ProximityShield);
                    mi.size = 32;
                }
                if (omp != null && omp.Mods != null) {
                    if (omp.Mods.Count > 2) {
                        mi.size = Math.Clamp(mi.size + 4 + (omp.Mods.Count - 4), 16, 25);
                        mi.priority = IconPriority.VeryHigh;
                        if (e.rarity == Rarity.Normal) {
                            mi.uv = sh.GetUV(MapIconsIndex.BestiaryBloodAlter);
                        }
                        else if (e.rarity == Rarity.Magic) {
                            mi.uv = sh.GetUV(MapIconsIndex.BestiaryBlueMonster);
                        }
                        else if (e.rarity == Rarity.Rare) {
                            mi.uv = sh.GetUV(MapIconsIndex.BestiaryYellowBeast);
                        }
                        else if (e.rarity == Rarity.Unique) {
                            mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                        }
                    }
                    if (omp.Mods.Any(m => m.Contains("ArchnemesisFrostTouched")))
                        mi.uv = sh.GetUV(MapIconsIndex.frost);
                    else if(omp.Mods.Any(m => m.Contains("MonsterArchnemesisVolatileFlameBlood")))
                        mi.uv = sh.GetUV(MapIconsIndex.FlameBlood);
                    else if(omp.Mods.Any(m => m.Contains("HeraldOfTheObelisk") || m.Contains("LivingCrystals")))
                        mi.uv = sh.GetUV(MapIconsIndex.Heralding_minions);
                    else if (omp.Mods.Contains("FlameWalker"))
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                    else if (omp.Mods.Contains("MonsterMapBoss"))
                        mi.uv = sh.GetUV(MapIconsIndex.BestiaryBoss);
                    else if (omp.Mods.Any(a => a.Contains("MonsterAura") || a.Contains("CannotBeStunned") || a.Contains("MonsterFastRun")))
                        mi.uv = sh.GetUV(MapIconsIndex.AuraFasterRuner);
                    else if (omp.Mods.Any(a => a.Contains("Bloodlines") || a.Contains("CorruptedBlood")))
                        mi.uv = sh.GetUV(MapIconsIndex.bleed);
                    else if (e.RenderName.Contains("Vampiric"))
                        mi.uv = sh.GetUV(MapIconsIndex.Vampiric);
                    else if (e.RenderName.Contains("Cavestalker"))
                        mi.uv = sh.GetUV(MapIconsIndex.Cavestalker);
                }
            }
            return mi;//MonsterIcon
        }
    }
}
