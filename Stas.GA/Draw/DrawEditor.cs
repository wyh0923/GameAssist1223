#region using
using System;
using System.Linq;
using ImGuiNET;
using Color = System.Drawing.Color;
#endregion
namespace Stas.GA {
    partial class DrawMain {
        void DrawEditor() {
            ImGui.PushItemWidth(40);
            if (ImGui.InputText("id", ref input, 5, ImGuiInputTextFlags.EnterReturnsTrue)) {
                int res = -1;
                int.TryParse(input, out res);
                if (res > 0)
                    ui.curr_map.debug_id = res;
            }
            ImGuiExt.ToolTip("Entity.ID for debug with Mapper...");
            ImGui.SameLine();

            if (ImGui.Button("+Tile")) {
                ui.curr_map.AddImportantTile(ui.me.gpos);
            }
            ImGuiExt.ToolTip("adds the tile we are standing on to the list of important tiles for the current map");
            ImGui.SameLine();

            if (ImGui.SliderFloat("icon size", ref ui.sett.icon_size, 8, 20)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Change base icon size. it also depends on the degree of magnification of the map");

        }
    }
}
