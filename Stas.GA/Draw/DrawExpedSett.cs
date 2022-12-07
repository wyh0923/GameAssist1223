using ImGuiNET;
using Stas.POE.Core;
using Color = System.Drawing.Color;

namespace Stas.GA;
partial class DrawMain {
    void DrawExpedSett() {
        ImGui.SetNextItemWidth(150);
        if (ImGui.SliderInt("Radius", ref ui.exped_sett.radius_persent, 0, 100)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("Increased Explosive Radius(35% from tree)");
        ImGui.SetNextItemWidth(150);
        ImGui.SameLine();
        if (ImGui.SliderInt("Range", ref ui.exped_sett.range_persent, 0, 100)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("increased Explosive plascement range(40% from tree)");


        if (ImGui.Checkbox("Phys", ref ui.exped_sett.PhysImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("PhysImmune");
        ImGui.SameLine();

        if (ImGui.Checkbox("Fire", ref ui.exped_sett.FireImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("FireImmune");
        ImGui.SameLine();

        if (ImGui.Checkbox("Cold", ref ui.exped_sett.ColdImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ColdImmune");
        ImGui.SameLine();

        if (ImGui.Checkbox("Lightn", ref ui.exped_sett.LightningImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("LightningImmune");
        ImGui.SameLine();

        if (ImGui.Checkbox("Chaos", ref ui.exped_sett.ChaosImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ChaosImmune");

        if (ImGui.Checkbox("Ailment", ref ui.exped_sett.AilmentImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("AilmentImmune");
        ImGui.SameLine();

        //NEw line
        if (ImGui.Checkbox("Crit", ref ui.exped_sett.CritImmune)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("CritImmune");
        ImGui.SameLine();

        if (ImGui.Checkbox("Culling", ref ui.exped_sett.Culling)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("CullingStrikeTwentyPercent");
        ImGui.SameLine();

        if (ImGui.Checkbox("Corrupt", ref ui.exped_sett.CorruptedItems)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ExpeditionCorruptedItemsElite");
        ImGui.SameLine();

        if (ImGui.Checkbox("Regen", ref ui.exped_sett.Regen)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ElitesRegenerateLifeEveryFourSeconds");

        //NEw line
        if (ImGui.Checkbox("Block", ref ui.exped_sett.BlockChance)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("AttackBlockSpellBlockMaxBlockChance");
        ImGui.SameLine();

        if (ImGui.Checkbox("Resist", ref ui.exped_sett.MaxResistances)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ResistancesAndMaxResistances");
        ImGui.SameLine();

        if (ImGui.Checkbox("Leech", ref ui.exped_sett.NoLeech)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("CannotBeLeechedFrom");
        ImGui.SameLine();

        if (ImGui.Checkbox("Curse", ref ui.exped_sett.NoCurse)) {
            ui.exped_sett.Save();
        }
        ImGuiHelper.ToolTip("ImmuneToCurses");
        ImGui.SameLine();
    }
}