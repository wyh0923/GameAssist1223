using ImGuiNET;
using System.Drawing;

namespace Stas.GA;
partial class DrawMain {
    bool b_cant_draw_map;
    void DrawTabs() {
        void _draw_alert() {
            if (ImGui.BeginTabItem("PA")) {
                DrawAllert();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide preload alerts");
        }
        void _draw_log() {
            if (ImGui.BeginTabItem("Log")) {
                DrawLog(ui.log);
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide log lines");
        }
        if (ImGui.BeginTabBar("Tabs", ImGuiTabBarFlags.AutoSelectNewTabs)) {// | ImGuiTabBarFlags.Reorderable
         
            if (ui.sett.b_draw_log_first) {
                _draw_log();
                _draw_alert();
            }
            else {
                _draw_alert();
                _draw_log();
            }

            if (ImGui.BeginTabItem("Sett")) {
                DrawSettings();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide settings panel");

            if (ImGui.BeginTabItem("Loot")) {
                DrawLootSett();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide loot settings");

            //if (ImGui.BeginTabItem("Exped")) {
            //    DrawExpedSett();
            //    ImGui.EndTabItem();
            //}
            //ImGuiHelper.ToolTip("Show/hide exped settings");

            if (ImGui.BeginTabItem("Visual")) {
                DrawVisual();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide Shows the visual effects settings");

            if (ImGui.BeginTabItem("Debug")) {
                DrawDebugSett();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide debug setting");

           
            if (ImGui.BeginTabItem("same test")) {
                ui.me.GetComp<Life>(out var life);
                if (life != null) {
                    ImGui.Text("life=[" + life.Health.Current + "\\" + life.Health.Total + "]");
                    ImGui.Text("Es=[" + life.EnergyShield.Current + "\\" + life.EnergyShield.Total + "]");
                    ImGui.Text("Mana=[" + life.Mana.Current + "\\" + life.Mana.Total + "]");
                }

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
                //DrawTests();
                //TODO still crash in launching after only one call!!!
                var busy = ui.b_busy;
                ImGui.Text(ui.gui.b_busy_info);
                //ImGui.Text("left = [" + ui.gui.open_left_panel.IsValid + "] right=["+ ui.gui.open_right_panel.IsValid + "]");
                //ImGui.Text("b_busy=[" + ui.b_busy + "]... sise=[" + ui.gui.b_busy_info?.Length + "]");

                ImGui.EndTabItem();
            }
           
            ImGuiExt.ToolTip("The window is usually only visible if something is wrong");

            ImGui.EndTabBar();
        }

    }
}