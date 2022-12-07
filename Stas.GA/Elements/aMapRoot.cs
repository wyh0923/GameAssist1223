using ImGuiNET;
using System.Numerics;
namespace Stas.GA;

/// <summary>
///     Points to the Map UiElement.
/// </summary>
internal class aMapElemet : Element {
    internal aMapElemet(IntPtr address)  : base(address, "aMapElemet") { 
    }

    internal override void Tick(IntPtr ptr, string from=null) {
        base.Tick(ptr, from);
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<MapUiElementOffset>(this.Address);
        this.shift.X = data.Shift.X;
        this.shift.Y = data.Shift.Y;

        this.defaultShift.X = data.DefaultShift.X;
        this.defaultShift.Y = data.DefaultShift.Y;

        this.Zoom = data.Zoom;
    }
    
    protected override void CleanUpData() {
        base.CleanUpData();
        this.shift = default;
        this.defaultShift = default;
        this.Zoom = 0.5f;
    }
    private Vector2 defaultShift = Vector2.Zero;
    private Vector2 shift = Vector2.Zero;
    /// <summary>
    ///     Gets the value indicating how much map has shifted.
    /// </summary>
    public Vector2 Shift => this.shift;

    /// <summary>
    ///     Gets the value indicating shifted amount at rest (default).
    /// </summary>
    public Vector2 DefaultShift => this.defaultShift;

    /// <summary>
    ///     Gets the value indicating amount of zoom in the Map.
    ///     Normally values are between 0.5f  - 1.5f.
    /// </summary>
    public float Zoom { get; private set; } = 0.5f;

    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Shift {this.shift}");
        ImGui.Text($"Default Shift {this.defaultShift}");
        ImGui.Text($"Zoom {this.Zoom}");
    }
}