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

            if (ImGui.BeginTabItem("Editor")) {
                DrawEditor();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide map editor content");

            if (ImGui.BeginTabItem("Debug")) {
                DrawDebugSett();
                ImGui.EndTabItem();
            }
            ImGuiExt.ToolTip("Show/hide debug setting");

           
            if (ImGui.BeginTabItem("same test")) {
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