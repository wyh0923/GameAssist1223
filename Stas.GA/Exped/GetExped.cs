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
using ExileCore.Shared.Enums;
using Stas.POE.Core;
using System.Reflection;
#endregion
namespace Stas.GA {
    public partial class AreaInstance {
        List<Entity> exped_key_frame = new List<Entity>();
        List<Entity> exped_beams_frame = new List<Entity>();
        public StaticMapItem exped_detonator => static_items.Values.FirstOrDefault(i => i.m_type == miType.ExpedDeton);
        MapItem GetExped(Entity e, MapItem mi) {
            mi.info = pa_info(e);
            if (mi.info == "ExpeditionMarker") {
                return GetExpedMarker(e);
            }
            else if (mi.info == "ExpeditionDetonator") {
                if (e.IsTargetable)
                    return asStaticMapItem(e, miType.ExpedDeton, MapIconsIndex.ExpeditionDetonator);
                else
                    return null;
            }
            else if (mi.info.Contains("Entrance")) {
                return asStaticMapItem(e, miType.door, MapIconsIndex.Red_door);
            }
            else if (mi.info == "ExpeditionRelic") {
                return GetExpedRelic(e);
            }
            else if (mi.info.Contains("ExpeditionExplosive")) { //ExpeditionConnectorPole
                if (!mi.info.Contains("Fuse"))
                    exped_key_frame.Add(e);
                else
                    exped_beams_frame.Add(e);
                return null;
            }
            else if (mi.info.Contains("ExpeditionStash")) {
                mi.uv = sh.GetUV(MapIconsIndex.ExpeditionStash);
            }
            else if (e.eType == eTypes.Chest) {
                if (!e.IsTargetable)
                    return null;
                mi.uv = sh.GetUV(MapIconsIndex.BigChest);
            }
            else {//for debug only //"ExpeditionConnectorPole"
                if (ui.sett.b_show_unknow)
                    mi.uv = sh.GetUV(MapIconsIndex.unknow);
                else
                    return null;
            }
            mi.size = (int)EXT.Lerp(14, 25, (float)ui.sett.map_scale / 20);
            return mi;
        }
       
    }
    public class Remnant {
        public Dictionary<string, int> positive = new();
        public Dictionary<string, int> negative = new();

    }
}
