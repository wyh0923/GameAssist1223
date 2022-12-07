using ImGuiNET;
namespace Stas.GA; 

partial class DrawMain {
    void DrawAllert() {
        for (int i = 0; i < ui.alert?.DrawAlerts.Count; i++) {
            var line = ui.alert.DrawAlerts[i];
            var text = line.DisplayName;
            if (string.IsNullOrEmpty(text))
                text = "error alert";
            ImGui.PushStyleColor(ImGuiCol.Text, line.Color.ToSysNumV4());
            ImGui.Text(text);
            ImGui.PopStyleColor();
        }
    }
}
