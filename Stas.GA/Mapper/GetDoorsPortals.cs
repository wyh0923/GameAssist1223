using System.Collections.Concurrent;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using sh = Stas.GA.SpriteHelper;
namespace Stas.GA;
public partial class AreaInstance {
    MapItem GetPortal(Entity e) {
        e.GetComp<AreaTransition>(out var area);
        if (pa_info(e).EndsWith("Portal")) {
            return asStaticMapItem(e, miType.portal, MapIconsIndex.Portal);
        }
       
        if (area != null) {
            var a_info = e.RenderName.Length > 0 ? e.RenderName : pa_info(e);
            if (pa_info(e).StartsWith("IncursionPortal"))
                return asStaticMapItem(e, miType.IncursionPortal, MapIconsIndex.IncursionPortal); //, e.IsTargetable, transit.Flag1 == 3 && transit.Flag2 == 0
            else
                return asStaticMapItem(e, miType.transit, MapIconsIndex.Entrance); //, !e.IsTargetable
        }
        ui.AddToLog("GetPortal err - no condition", MessType.Error);
        return null;
    }
    MapItem GetDoor(Entity e) {
        e.GetComp<TriggerableBlockage>(out var _trigger);
        e.GetComp<Targetable>(out var target);
        //heist these doors can be filtered with:  && !ent.HasComponent<MinimapIcon>()
        if (e.Path.ToLower().Contains("door")) { //doors
            var ii = MapIconsIndex.Green_door;
            if (_trigger.IsBlocked) {
                ii = MapIconsIndex.Red_door;
            }
            return asStaticMapItem(e, miType.door, ii); //_trigger.IsClosed
        }
        //"Metadata/Terrain/Labyrinth/Objects/Puzzle_Parts/Switch"
        if (_trigger == null && e.Path.ToLower().Contains("switch")
            && target.isTargetable && e.eType != eTypes.Monster) { //switch
            return asStaticMapItem(e, miType.@switch, MapIconsIndex.LabyrinthLever); //!e.IsTargetable
        }
        if (e.GetComp<Portal>(out _) && e.GetComp<Transitionable>(out _)) {
            if (!e.IsTargetable) //used portals
                return null;
        }
        ui.AddToLog("GetDoor err - no condition", MessType.Error);
        return null;
    }
}