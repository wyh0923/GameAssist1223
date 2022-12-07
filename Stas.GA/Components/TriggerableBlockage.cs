using System;
using System.Drawing;
using ImGuiNET;

namespace Stas.GA; 

/// <summary>
///     The <see cref="TriggerableBlockage" /> component in the entity.
/// </summary>
public class TriggerableBlockage : EntComp {
    public TriggerableBlockage(IntPtr address): base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<TriggerableBlockageOffsets>(Address);
        IsBlocked = data.IsBlocked;
        owner_addr = new IntPtr(data.Header.EntityPtr);
        Min = new Point(ui.m.Read<int>(Address + 0x50), ui.m.Read<int>(Address + 0x54));
        Max = new Point(ui.m.Read<int>(Address + 0x58), ui.m.Read<int>(Address + 0x5C));
    }
    public IntPtr owner_addr { get; private set; } = IntPtr.Zero;
    public Point Min { get; private set; }
    public Point Max { get; private set; }
   
    /// <summary>
    ///     Gets a value indicating whether TriggerableBlockage is closed or not.
    /// </summary>
    public bool IsBlocked { get; private set; }

    /// <summary>
    ///     Converts the <see cref="Chest" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Is Blocked: {this.IsBlocked}");
    }

    /// <inheritdoc />
    protected override void CleanUpData() {
        throw new Exception("Component Address should never be Zero.");
    }
}