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
using ExileCore.PoEMemory.Components;
namespace Stas.GA;
#endregion
public partial class AreaInstance {
    MapItem GetExpedMarker(Entity e) {
        e.GetComp<Animated>(out var animated);
        e.GetComp<Render>(out var render);
        var anim = animated.BaseAnimatedObjectEntity.Metadata;
        if (anim == null)
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.unknow, "Error", IconPriority.High);

        if (anim.Contains("Terrain/Leagues/Expedition/Tiles")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.unknow,
                "Tiles", IconPriority.Low);
        }
        else if (anim.Contains("ChestDelirium")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardChestDelirium,
           "Delirium", IconPriority.High);
        }
        else if (anim.Contains("ChestGems")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardGems,
           "Gems", IconPriority.High);
        }
        else if (anim.Contains("ChestEssence")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Essence,
           "Essence", IconPriority.High);
        }
        else if (anim.Contains("ChestBreach")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardBreach,
           "Breach", IconPriority.High);
        }
        else if (anim.Contains("ChestLegion")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.LegionGeneric,
           "Legion", IconPriority.High);
        }
        else if (anim.Contains("elitemarker")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_elitemarker,
                "Elit", IconPriority.High);
        }
        else if (anim.Contains("ChestFossils")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardFossils,
                "Fossils", IconPriority.High);
        }
        else if (anim.Contains("ChestMetamorph")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardChestMetamorph,
                "Metamorph", IconPriority.High);
        }
        else if (anim.Contains("ChestCurrency")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardCurrency,
                "Currency", IconPriority.High);
        }
        else if (anim.Contains("ChestFragments")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardFragments,
                "Fragments", IconPriority.High);
        }
        else if (anim.Contains("ChestDivinationCards")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardDivinationCards,
                "Divination", IconPriority.High);
        }
        else if (anim.Contains("ChestUniques")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardUniques,
                "Uniques", IconPriority.High);
        }
        else if (anim.Contains("ChestTrinkets")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Trinked,
                "Trinkets", IconPriority.High);
        }
        else if (anim.Contains("ChestWeapon")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardWeapons,
                "Weapon", IconPriority.High);
        }
        else if (anim.Contains("ChestHarbinger")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardHarbinger,
                "Harbinger", IconPriority.High);
        }
        else if (anim.Contains("ChestHeist")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.RewardHeist,
                "Heist", IconPriority.High);
        }
        else if (anim.Contains("ChestLeague")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_chest_multy,
                "ChestLeague", IconPriority.Low);
        }
        else if (anim.Contains("monstermarker")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_monstermarker,
                "monstr", IconPriority.Low);
        }
        else if (anim.Contains("chestmarker3")) { //unic chest marker
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_chest3,
                "chestmarker3", IconPriority.Low);
        }
        else if (anim.Contains("chestmarker2")) { //rare chest marker
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_chest2,
                "chestmarker2", IconPriority.Low);
        }
        else if (anim.Contains("chestmarker1")) {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.unknow,
                "chestmarker1", IconPriority.Low);
        }
        else if (anim.Contains("chestmarker_signpost")) {//white chest marker
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.Exped_chest,
                "signpost", IconPriority.Low);
        }
        else {
            return asStaticMapItem(e, miType.ExpedMarker, MapIconsIndex.unknow,
                anim, IconPriority.Low);
        };
    }
}
