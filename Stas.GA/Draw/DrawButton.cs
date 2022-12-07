using Color = System.Drawing.Color;
using ImGuiNET;
namespace Stas.GA;

partial class DrawMain {
    void DrawButtons() {
        foreach (var bd in ui.buttons.Values) { 
            ImGui.Begin(bd.key.ToString(), ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.AlwaysAutoResize);
            DrawButton(bd);
            ImGui.End();
        }
    }
    void DrawButton(aButtonDebug bd) {
        var bc = Color.Gray.ToImguiVec4();
        var b_down = Mouse.IsButtonDown(bd.key);
        if (b_down)
            bc = Color.Red.ToImguiVec4();
        ImGui.PushStyleColor(ImGuiCol.Button, bc);
        if(ImGui.Button(bd.key+": "+bd.down_count+" / " + bd.up_count)) {
        }
        ImGui.PopStyleColor();
        DrawLog(bd.log);
    }
}
