using ImGuiNET;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

/// <summary>
///     The <see cref="Positioned" /> component in the entity.
/// </summary>
public class Positioned : EntComp {
    public Positioned(IntPtr address)  : base(address) {
    }
   
    internal override void Tick(IntPtr ptr, string from=null) {
        this.Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<PositionedOffsets>(this.Address);
        var gp = data.GridPosition;
        GridPos = new V2(gp.X, gp.Y);
        Reaction = data.Reaction;
        IsFriendly = EntityHelper.IsFriendly(data.Reaction);
        Rotation = data.Rotation;
        Size = data.Size;        
        Scale = data.SizeScale;
        past_pos = data.past_pos;
        curr_pos = data.curr_pos;
        next_pos = data.next_pos;
        //var less = past_pos.less_or_equal(curr_pos);
        //var greater = next_pos.greater_or_equal(curr_pos);
      
        //if (e_test!=null && owner_ptr == e_test.Address) {
        //    var me2p = new V2(ui.me.Pos.X, ui.me.Pos.Y);
        //    var epos = e_test.Pos;
        //    var past =  past_pos.GetDistance(me2p) >= curr_pos.GetDistance(me2p);
        //    var next = next_pos.GetDistance(me2p) <= curr_pos.GetDistance(me2p);
        //    ui.AddToLog("past=[" + past + "] next=[" + next + "] ", MessType.OnTop);
        //}
        
    }
    /// <summary>
    /// in radians
    /// </summary>
    public float Rotation { get; private set; }
    /// <summary>
    /// in degrees
    /// </summary>
    public float RotationDeg => Rotation * (float)(180 / Math.PI);
    public float Size { get; private set; }
    public float Scale { get; private set; }
    public V2 GridPos { get; private set; }
    public V2 past_pos { get; private set; }
    public V2 curr_pos { get; private set; }
    public V2 next_pos { get; private set; }
    /// <summary>
    /// entity is friendly or not.
    /// </summary>
    public byte Reaction { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the entity is friendly or not.
    /// </summary>
    public bool IsFriendly { get; private set; }

    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Reaction: {this.Reaction:X}");
        ImGui.Text($"IsFriendly: {this.IsFriendly}");
    }
}