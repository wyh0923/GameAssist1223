#region using
using System;
using System.Linq;
using ImGuiNET;
using Color = System.Drawing.Color;
#endregion
namespace Stas.GA {
    partial class DrawMain {
        void DrawLootSett() {
            if (ImGui.Checkbox("Quest", ref ui.looter.sett.b_quest)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGuiExt.ToolTip("Collect quest items [def=ON]");
            ImGui.SameLine();

            if (ImGui.Checkbox("League", ref ui.looter.sett.b_league)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGuiExt.ToolTip("Get league mechanics loot [def=ON]");
            ImGui.SameLine();

            if (ImGui.Checkbox("Map", ref ui.looter.sett.b_maps)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Currency", ref ui.looter.sett.b_currency)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGuiExt.ToolTip("Get any Currency [def=off]");
            ImGui.SameLine();

            if (ImGui.Checkbox("Portal", ref ui.looter.sett.b_portal)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Wisdom", ref ui.looter.sett.b_wisdom)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            //=>next line

            if (ImGui.Checkbox("Transm", ref ui.looter.sett.b_transmut)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Augm", ref ui.looter.sett.b_augment)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Alter", ref ui.looter.sett.b_alter)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("AScrap", ref ui.looter.sett.b_arm_scrap)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("WStone", ref ui.looter.sett.b_whetstone)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            //=>next line

            if (ImGui.Checkbox("Claw", ref ui.looter.sett.b_Claw)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Wand", ref ui.looter.sett.b_Wand)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Dagger", ref ui.looter.sett.b_Dagger)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Body", ref ui.looter.sett.b_Body_Armour)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Boots", ref ui.looter.sett.b_Boots)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            //=>next line

            if (ImGui.Checkbox("Gloves", ref ui.looter.sett.b_Gloves)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Helmet", ref ui.looter.sett.b_Helmet)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Amulet", ref ui.looter.sett.b_Amulet)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Belt", ref ui.looter.sett.b_Belt)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
            if (ImGui.Checkbox("Ring", ref ui.looter.sett.b_Ring)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            //=>next line

            if (ImGui.Checkbox("bow", ref ui.looter.sett.b_small_bow)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGuiExt.ToolTip("Short Bow 2x3 as veapon for Chaos Set");
            ImGui.SameLine();

            if (ImGui.Checkbox("6s big", ref ui.looter.sett.b_6s_big)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("6s small", ref ui.looter.sett.b_6s_small)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("6 ling", ref ui.looter.sett.b_6l_any)) {
                ui.looter.sett.Save();
                ui.looter.LoadSett();
            }
            ImGui.SameLine();
        }
    }
}
