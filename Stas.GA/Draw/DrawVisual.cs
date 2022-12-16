#region using
using System;
using System.Linq;
using ImGuiNET;
using Color = System.Drawing.Color;
#endregion
namespace Stas.GA {
    partial class DrawMain {
        void DrawVisual() {
            ImGui.SetNextItemWidth(60);
            if (ImGui.SliderFloat("icons", ref ui.sett.icon_size, 8, 20)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Change base icon size. it also depends on the degree of magnification of the map");

            ImGui.SameLine();
            ImGui.SetNextItemWidth(60);
            if (ImGui.SliderInt("Visited", ref ui.sett.visited_persent, 5, 30)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("the transparency regulator for the seeded areas - I will make the color later");

           
            //================> new line
            ImGui.SetNextItemWidth(60);
            if (ImGui.Checkbox("PlPos", ref ui.sett.b_draw_me_pos)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Draw red line behind the hero \n" +
                "to visualize FPS and latency in one place ");

            ImGui.SetNextItemWidth(60);
            ImGui.SameLine();
            if (ImGui.SliderFloat("Font", ref ui.sett.info_font_size, 1, 2)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Font size for this UI.info panel. default=1");

            if (ImGui.Button("+Tile")) {
                ui.curr_map.AddImportantTile(ui.me.gpos);
            }
            ImGuiExt.ToolTip("adds the tile we are standing on " +
                "\nto the list of important tiles for the current map");

            ImGui.SameLine();
            ImGui.Checkbox("Tile", ref ui.b_tile);
            ImGuiExt.ToolTip("When you press Alt, it shows the title under your feet.\nIf it is unique, you can add it \nto the important tiles on this type of map)");

        }
    }
}
