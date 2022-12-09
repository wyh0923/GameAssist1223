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
            if (ImGui.SliderInt("PlPos", ref ui.sett.max_player_debug_pos, 0, 256)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("the red line behind the hero - visual fps and dwell time at one point");

            ImGui.SetNextItemWidth(60);
            ImGui.SameLine();
            if (ImGui.SliderFloat("Font", ref ui.sett.info_font_size, 1, 2)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Font size for this UI.info panel. default=1");
        }
    }
}
