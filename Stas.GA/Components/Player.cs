using ImGuiNET;
namespace Stas.GA; 
public class Player : EntComp{
    public Player(IntPtr address)   : base(address) { 
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<PlayerOffsets>(this.Address);
        Name = ui.m.ReadStdWString(data.Name);
    }
    /// <summary>
    ///     Gets the name of the player.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Converts the <see cref="Player" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui()
    {
        base.ToImGui();
        ImGui.Text($"Player Name: {this.Name}");
    }
}