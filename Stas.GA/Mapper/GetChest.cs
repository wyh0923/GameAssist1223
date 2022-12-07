using System.Collections.Concurrent;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
using Stas.GA;
using System.Diagnostics;

namespace Stas.GA;
public partial class AreaInstance {
    string[] can_distr = new string[] // any destructive
       { "Barrel", "Bloom", "Vase", "Pile", "Pot", "Crate", "Basket", "Cairn", "Urn", "Egg", "Boulder", "WormJar"};
    MapItem GetChest(Entity e, MapItem mi) {
        e.GetComp<Chest>(out var chest);
        var key = e.GetKey; //1336988036
        if (key == 1336988036) {
        }
        if (e.IsOpened) {
            static_items.TryRemove(key, out _);
            return null;
        }
        mi.size = 20;
        var smi = new StaticMapItem(e, miType.Chest, mi.info);
        smi.uv = sh.GetUV(MapIconsIndex.StashGuild);
        if (e.eType == eTypes.Barrel) { //can_distr.Any(d => e.Path.Contains(d) && e.Path.Contains("Chests") && !e.Path.Contains("City") && !e.Path.Contains("Cave"))
            smi.uv = sh.GetUV(MapIconsIndex.Barrel);
            smi.size = 10;
            smi.m_type = miType.Barrel;
        }
        else { //order rted here
            if (mi.info.EndsWith("Strongbox")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardNiceBox);
            }
            else if (mi.info.Contains("Flare")) {
                smi.uv = sh.GetUV(MapIconsIndex.Flare);
            }
            else if (e.Path.Contains("LegionChests")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardNiceBox);
            }
            else if (mi.info.Contains("Wealth") || mi.info.Contains("Riches")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardCurrency);
            }
            else if (mi.info == "Excavated Chest" || mi.info.Contains("Loot")
                || mi.info == "Blight Cyst") {
                smi.uv = sh.GetUV(MapIconsIndex.RewardGeneric);
            }
            else if (mi.info.Contains("Armaments") || mi.info.Contains("Armour")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardArmour);
            }
            else if (mi.info.Contains("Jewels")) {
                smi.uv = sh.GetUV(MapIconsIndex.Heist_Jewels);
            }
            else if (mi.info.Contains("Lockbox")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardNiceBox);
            }
            else if (mi.info.Contains("Curio Display")) {
                smi.uv = sh.GetUV(MapIconsIndex.CurioDisplay);
            }
            else if (mi.info.Contains("Jewellery") || mi.info.Contains("Rings")) {
                smi.uv = sh.GetUV(MapIconsIndex.Ring);
            }
            else if (mi.info.Contains("Azurite")) {
                smi.uv = sh.GetUV(MapIconsIndex.Azurite);
            }
            else if (e.Path.Contains("Trinkets"))
                smi.uv = sh.GetUV(MapIconsIndex.AmuletAndRing);
            else if (e.Path.Contains("Armour"))
                smi.uv = sh.GetUV(MapIconsIndex.RewardArmour);
            else if (e.Path.Contains("BlightInfectedMaps")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardMaps);
            }
            else if (e.Path.Contains("Currency")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardCurrency);
            }
            else if (e.Path.Contains("Weapon"))
                smi.uv = sh.GetUV(MapIconsIndex.RewardWeapons);
            else if (e.Path.Contains("Resonator")) {
                smi.uv = sh.GetUV(MapIconsIndex.Resonator);
            }
            else if (e.Path.Contains("JaggedFossilChest")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardFossils);
            }
            else if (e.Path.Contains("OffPathArmour")) {
                smi.uv = sh.GetUV(MapIconsIndex.RewardArmour);
            }
            else if (pa_info(e).Contains("AzuriteVein")) {
                smi.uv = sh.GetUV(MapIconsIndex.Azurite);
            }
            else if (pa_info(e).Contains("Dynamite")) {
                smi.uv = sh.GetUV(MapIconsIndex.Dynamite);
            }
            else if (pa_info(e).Contains("Flares")) {
                smi.uv = sh.GetUV(MapIconsIndex.Flare);
            }
            else if (e.Path.Contains("AbyssFinalChest")) {
                mi.uv = sh.GetUV(MapIconsIndex.RewardNiceBox);
            }

        }


        smi.priority = IconPriority.High;
        static_items[smi.key] = smi;
        return null;
    }
}