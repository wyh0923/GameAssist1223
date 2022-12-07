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
        MapItem GetPlayer(Entity e, MapItem mi) {
            e.GetComp<Player>(out var player);
            var pn = player?.Name;
            if (string.IsNullOrEmpty(pn))
                pn = "Unknow!";
            if (e.Address == ui.me.Address) {
                mi.info = "Me";
                mi.size = 14;
                mi.uv = sh.GetUV(MapIconsIndex.PartyLeader);
                mi.priority = IconPriority.Critical;
            }
            else {
                if (ui.curr_role == Role.Master) {
                    var bot = ui.bots.FirstOrDefault(b => b.bot_name == pn);
                    if (bot != null) {
                        bot.ent = e;
                        return null; //we will draw it like statck map item
                    }
                }
                mi.info = pn;
                mi.uv = sh.GetUV(MapIconsIndex.OtherPlayer); //PlayerIcon
                mi.priority = IconPriority.Medium;
            }
            return mi;
        }
    }
}
