using ImGuiNET;
namespace Stas.GA;
partial class DrawMain {
    void DrawDebugInfo() {
        if (ImGui.BeginTabItem("same test")) {
            //ui.AddToLog("offs=[" + ui.map_offset.ToString() + "]");
            //ui.AddToLog("ui.b_busy=[" + ui.b_busy + "]");
            //ui.AddToLog("me.pos=["+ui.me.Pos.ToIntString()+"]");
            //ui.AddToLog("local=[" + ui.gui.chat_box_elem.GetTextElem_by_Str("Local") + "]");
            ImGui.Text("my map res==[" + my_display_res.ToIntString() + "] scale=["+ EXT.GetScreenScalingFactor() + "]");
            if (b_cant_draw_map) {
                ImGui.Text("ui.nav.b_read=[" + ui.nav.b_ready + "]");
                ImGui.Text("b_map=[" + b_map + "]");
                ImGui.Text("on_top=[" + on_top + "]");
                ImGui.Text("b_busy=[" + ui.b_busy + "]");
                ImGui.Text("ui.sett.b_draw_bad_centr=[" + ui.sett.b_draw_bad_centr + "]");
                ImGui.Text("ui.b_draw_save_screen=[" + ui.b_draw_save_screen + "]");
            }

            ImGui.PushItemWidth(40);
            if (ImGui.InputText("id", ref input, 5, ImGuiInputTextFlags.EnterReturnsTrue)) {
                int res = -1;
                int.TryParse(input, out res);
                if (res > 0)
                    ui.curr_map.debug_id = res;
            }
            ImGuiExt.ToolTip("Entity.ID for debug with Mapper...");
         
          
            //DrawTests();
            //TODO still crash in launching after only one call!!!
            var busy = ui.b_busy;
            ImGui.Text(ui.gui.b_busy_info);
            //ImGui.Text("left = [" + ui.gui.open_left_panel.IsValid + "] right=["+ ui.gui.open_right_panel.IsValid + "]");
            //ImGui.Text("b_busy=[" + ui.b_busy + "]... sise=[" + ui.gui.b_busy_info?.Length + "]");

            ImGui.EndTabItem();
        }
        ImGuiExt.ToolTip("Same debug infarmation here");
    }
    void DrawLife() {
        ui.me.GetComp<Life>(out var life);
        if (life != null) {
            ImGui.Text("life=[" + life.Health.Current + "\\" + life.Health.Total + "]");
            ImGui.Text("Es=[" + life.EnergyShield.Current + "\\" + life.EnergyShield.Total + "]");
            ImGui.Text("Mana=[" + life.Mana.Current + "\\" + life.Mana.Total + "]");
        }
    }
} 