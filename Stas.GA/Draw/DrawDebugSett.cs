﻿#region using
using System;
using System.Linq;
using ImGuiNET;
using Color = System.Drawing.Color;
#endregion
namespace Stas.GA {
    partial class DrawMain {
       
        void DrawDebugSett() {
            if (ImGui.Checkbox("Debug", ref ui.sett.b_debug)) {
                //ui.log.Clear();
                //ui.tasker.Reset("b_debug");
            }
            ImGuiExt.ToolTip("all autotask disabled");

            ImGui.SameLine();
            if (ImGui.Checkbox("SaveScreen", ref ui.b_draw_save_screen)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("draw part of the screen where you can safely move the mouse");

            ImGui.SameLine();
            if (ImGui.Checkbox("BadCentr", ref ui.sett.b_draw_bad_centr)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("draw bad center area");

            ImGui.SameLine();
            if (ImGui.Checkbox("Fps", ref ui.sett.b_draw_map_fps)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("show map drawing fps - optimal value (for drawing without jerks) above 2500 fps");


            ImGui.SameLine();
            if (ImGui.Checkbox("Develop", ref ui.sett.b_develop)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("puts the editor into developer mode\r\nShows a lot of garbage for debugging\r\nNot necessary for ordinary users");
            
            ///===================== new line ====================
            if (ImGui.Checkbox("MouseMoving", ref ui.sett.b_draw_mouse_moving)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("draw mouse moving traces");

            ImGui.SameLine();
            ImGui.Checkbox("Tile", ref ui.b_tile);
            ImGuiExt.ToolTip("Show tiles with Get_Tile_Name_by_my_gpos()");

            ImGui.SameLine();
            ImGui.Checkbox("Cells", ref ui.b_show_cell);
            ImGuiExt.ToolTip("Show Nav map cells...");

            ImGui.SameLine();
            if (ImGui.Checkbox("Misk", ref ui.sett.b_draw_misk)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("draw misks as debug enetity(alot spam on map)");

            ImGui.SameLine();
            if (ImGui.Checkbox("Over", ref ui.sett.b_show_info_over)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("draw this[GA.Info panel] alvays over(for debug samping mb)[Ctrl+F12]");
            
            ImGui.SameLine();
            if (ImGui.Checkbox("Tgr", ref ui.sett.b_show_iTask)) {
                ui.sett.Save();
            }
            ImGuiExt.ToolTip("Chow action trg");

            //if (ImGui.Button("SetGameTop")) {
            //    ui.SetTop(ui.game_ptr, 500);
            //}
            //ImGuiHelper.ToolTip("testing SetTop");
        }
    }
}
