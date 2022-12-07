using ImGuiNET;
namespace Stas.GA; 

/// <summary>
///     The <see cref="Charges" /> component in the entity.
/// </summary>
public class Charges : EntComp {
    /// <summary>
    ///     Initializes a new instance of the <see cref="Charges" /> class.
    /// </summary>
    /// <param name="address">address of the <see cref="Charges" /> component.</param>
    public Charges(IntPtr address) : base(address) { }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<ChargesOffsets>(this.Address);
        this.Current = data.current;
    }
    protected override void CleanUpData() {
        throw new Exception("Component Address should never be Zero.");
    }
    /// <summary>
    ///     Gets a value indicating number of charges the flask has.
    /// </summary>
    public int Current { get; private set; }

    
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Current Charges: {this.Current}");
    }
}