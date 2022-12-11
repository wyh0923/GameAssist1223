using ImGuiNET;
namespace Stas.GA;
partial class DrawMain {
    void DrawDebugInfo() {
        if (ImGui.BeginTabItem("same test")) {
            //ui.AddToLog("offs=[" + ui.map_offset.ToString() + "]");
            //ui.AddToLog("ui.b_busy=[" + ui.b_busy + "]");
            //ui.AddToLog("me.pos=["+ui.me.Pos.ToIntString()+"]");
            //ui.AddToLog("local=[" + ui.gui.chat_box_elem.GetTextElem_by_Str("Local") + "]");
            ImGui.Text("my map res==[" + my_display_res.ToIntString() + "]");

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
        ImGuiExt.ToolTip("Same debug infarmation here");
    }
} 