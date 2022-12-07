using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using V2 = System.Numerics.Vector2;
using sh = Stas.GA.SpriteHelper;
using System.Runtime.CompilerServices;

namespace Stas.GA;
public partial class AreaInstance {
    MapItem GetIcon(Entity e, MapItem mi) {
        if (!e.GetComp<MinimapIcon>(out var icon) || icon.owner_ptr != e.Address) {
            ui.AddToLog("MinimapIcon is invalid", MessType.Error);
            return null;
        }
        var name = icon.name;
        //if (e.eType == eTypes.LegionMonolith && !e.IsTargetable) //
        //    return null;
        if (e.eType == eTypes.Chest && e.IsOpened)
            return null;

        if (!string.IsNullOrEmpty(name)) {
            if (name == "DynamiteWall") {
                return asStaticMapItem(e, miType.DynamiteWall, MapIconsIndex.DynamiteWall);
            }
            if (name == "LakeStampingDevice") {
                return asStaticMapItem(e, miType.LeagueEvent, MapIconsIndex.LakeStampingDevice);
            }
            if (name == "StashPlayer") {
                return asStaticMapItem(e, miType.Stash, MapIconsIndex.StashPlayer);
            }
            if (name == "StashGuild") {
                return asStaticMapItem(e, miType.GuildStash, MapIconsIndex.StashGuild);
            }
            if (name.StartsWith("HarvestCollector")) {
                return asStaticMapItem(e, miType.Harvest, MapIconsIndex.HarvestCollectorBlue); //, !e.IsTargetable
            }
            if (name == "UltimatumAltar") {
                return asStaticMapItem(e, miType.Ultimatum, MapIconsIndex.UltimatumAltar); //, !e.IsTargetable
            }
            if (name.Contains("RitualRune")) {
                return asStaticMapItem(e, miType.Ritual, MapIconsIndex.RitualRewards); //, e.buffs != null
            }
            if (e.Path.Contains("DelveMineral")) {
                if (!e.IsTargetable)
                    return null;
                var info = "Sulphite id=" + e.id + " gdist=" + e.gdist_to_me.ToRoundStr(0);
                return asStaticMapItem(e, miType.Sulphite, MapIconsIndex.Sulphite, info); //, !e.IsTargetable
            }
            if (e.Path.Contains("BlightPump")) {
                blight_pamp = e;
                return asStaticMapItem(e, miType.Blight, MapIconsIndex.BlightSpawner); //, !e.IsTargetable
            }
            //if (name == "BlightPathInactive")
            //    return null;
            if (name == "BlightPath" || name == "BlightPathInactive") {
                if (e.GetComp<Beam>(out var beam)) {
                    frame_blight.Add(e);
                }
            }
            var iconIndexByName = ui.IconIndexByName(name);
            if (iconIndexByName != 0) {
                #region Standard icon name - can be found in iconList
                mi.uv = sh.GetUV(iconIndexByName);
                mi.size = 16;
                //if (name.StartsWith("Heist") || name.Contains("Stash")
                //    || name.Contains("HarvestCollector") || e.League == LeagueType.Expedition) {
                //    mi.size = 16;
                //}
                //if (e.eType == eTypes.AreaTransition) {
                //    throw new NotImplementedException("all portal must be called below");
                //}
                if (e.eType == eTypes.Waypoint) {
                    var nsmi = new StaticMapItem(e, miType.waypoint);
                    nsmi.uv = mi.uv;
                    nsmi.size = mi.size;
                    static_items[nsmi.key] = nsmi;
                    return null;
                }
                #endregion
            }
            else { //icon Name NOT found in base icon collection 

                mi.size = 20;

                if (e.Path.Contains("BlightPath")) {
                    if (e.IsAlive)
                        mi.uv = sh.GetUV(MapIconsIndex.BlightPathActive);
                    else
                        mi.uv = sh.GetUV(MapIconsIndex.BlightPathInactive);
                    return mi;
                }
                if (name == "AfflictionInitiator") {
                    if (icon.IsHide)
                        return null;
                    mi.uv = sh.GetUV(MapIconsIndex.RewardChestDelirium);
                }
                if (name == "HeistEngineering") {
                    mi.uv = sh.GetUV(MapIconsIndex.CraftingBench);
                }
                if (name == "HeistCounterThaumaturge") {
                    if (e.IsDead)
                        return null;
                }
                if (name == "HeistSpottedMiniBoss") {
                    if (e.IsDead)
                        return null;
                    mi.uv = sh.GetUV(MapIconsIndex.BlightBoss);
                }
                if (name == "HeistDisplayCase")
                    mi.uv = sh.GetUV(MapIconsIndex.DelveMapViewer);
                if (name == "HeistPathChest") {
                    mi.info = e.Path.Replace("Metadata/Chests/LeagueHeist/HeistChestSecondary", "");
                }
                if (name.Contains("Jewellery") || e.Path.Contains("Jewellery") || e.Path.Contains("Jewels")
                    || name.Contains("Trinkets") || e.Path.Contains("Trinkets")) {
                    mi.uv = sh.GetUV(MapIconsIndex.Ring);
                }

                if (name.Contains("Currency") || e.Path.Contains("Currency")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardCurrency);
                }
                else if (name.Contains("Fragment") || e.Path.Contains("Fragment")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardFragments);
                }
                if (name.Contains("Unique") || e.Path.Contains("Unique")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardUniques);
                }
                if (name.Contains("Divination") || e.Path.Contains("Divination") || e.Path.Contains("StackedDecks")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardDivinationCards);
                }
                if (name.Contains("Weapons") || e.Path.Contains("Weapons")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardWeapons);
                }
                if (name.Contains("Harbinger") || e.Path.Contains("Harbinger")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardHarbinger);
                }
                if (name.Contains("Armour") || e.Path.Contains("Armour")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardArmour);
                }
                if (name.Contains("Essence") || e.Path.Contains("Essence")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardEssences);
                }
                if (name.Contains("Fossil") || e.Path.Contains("Fossil")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardFossils);
                }
                if (name.Contains("Prophecy") || name.Contains("Prophecies")
                    || e.Path.Contains("Prophecy") || e.Path.Contains("Prophecies")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardProphecy);
                }
                if (name.Contains("Gems") || e.Path.Contains("Gems")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardGems);
                }
                if (name.Contains("Corrupted") || e.Path.Contains("Corrupted")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardCorrupted);
                }
                if (name.Contains("Generic") || e.Path.Contains("Generic")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardGeneric);
                }
                if (name.Contains("Abyss") || e.Path.Contains("Abyss")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardAbyss);
                }
                if (name.Contains("Talisman") || e.Path.Contains("Talisman")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardTalisman);
                }
                if (name.Contains("Maps") || e.Path.Contains("Maps")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardMaps);
                }
                if (name.Contains("Breach") || e.Path.Contains("Breach")) {
                    mi.uv = sh.GetUV(MapIconsIndex.RewardBreach);
                }
            }
        }
        else {
           
            mi.size = 20;
            if (e.Path.Contains("LabyrinthTrialReturnPortal")) {
                mi.uv = sh.GetUV(MapIconsIndex.EntrancePortal);
            }
            else if (e.Path.Contains("MappingDevice")) {
                mi.uv = sh.GetUV(MapIconsIndex.MapDevice);
            }
            else if (e.Path.Contains("LeagueBetrayal")) {
                if (e.Path.Contains("MasterNinjaCop")) {
                    mi.uv = sh.GetUV(MapIconsIndex.BetrayalSymbolDjinn);
                }
            }
            else if (e.Path.Contains("HeistStash_Hideout")) {
                mi.info = "HeistStash";
                mi.uv = sh.GetUV(MapIconsIndex.HeistStockpile);
            }
            else if (e.Path.Contains("HeistPatrolMinimapIcon")) {
                mi.info = "Patrol";
                mi.uv = sh.GetUV(MapIconsIndex.BlightBoss);
            }
            else if (e.Path.Contains("HeistChest")) {
                if (e.Path.Contains("Delve")) {
                    mi.info = "Heist DElve Stash";
                    mi.uv = sh.GetUV(MapIconsIndex.RewardFossils);
                }
                else {
                    ui.AddToLog("Not found: {HeistChest}" + e.ToString());
                }
            }
            else {
                bad_map_items[mi.ent.Address.ToString("X")] = mi;
                //ui.AddToLog("GetIcon err=>not found...", MessType.Error);
            }
        }
        return mi;
    }
    
    
}
