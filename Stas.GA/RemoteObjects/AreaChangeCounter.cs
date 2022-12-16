using ImGuiNET;

namespace Stas.GA;
/// <summary>
/// this 'value' need for preload for cheking version of files
/// </summary>
[CodeAtt("Must be updated after each map change")]
public class AreaChangeCounter : RemoteObjectBase {//core.
    internal AreaChangeCounter(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        Value = ui.m.Read<AreaChangeOffset>(Address).counter;
    }

    protected override void Clear() {
        Value = int.MaxValue;
    }
    /// <summary>
    ///     Gets the cached value of the AreaChangeCounter.
    /// </summary>
    public int Value { get; private set; } = int.MaxValue;

    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Area Change Counter: {Value}");
    }
}