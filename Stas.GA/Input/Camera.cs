using System.Numerics;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using ImGuiNET;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Stas.GA;

public partial class Camera : RemoteObjectBase {
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetCameraOffsets")]
    public static extern int GetCameraOffsets(IntPtr cam_ptr, IntPtr ingame_state_ptr,  ref CameraOffsets offs);

    public Camera() : base(IntPtr.Zero) {
    }
    CameraOffsets data = new CameraOffsets();
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        GetCameraOffsets(ptr, ui.states.ingame_state.Address, ref data);
        curs_to_world = ui.m.Read<V2>( data.curs_ptr);
        Width = data.Width;
        Height = data.Height;
        me_pos = data.me_pos; //its NOT same like ui.me.pos but close - need check why
        worldToScreenMatrix = data.MatrixBytes;
        var target_addr = data.target_ent_ptr;
        if (target_addr == IntPtr.Zero)
            target_ent = null;
        else {
            if (target_ent == null)
                target_ent = new Entity(target_addr);
            else
                target_ent.Tick(target_addr);
        }
        //SameTest(); pass 3.19
    }
    public V3 me_pos;
    public V2 curs_to_world;
    public bool IsVaild => true;
    void DebugInfo() {
        if (target_ent == null)
            return;
        var _name = target_ent.RenderName;
        if (string.IsNullOrEmpty(_name))
            _name = target_ent.Path;
        ui.AddToLog("last Cam target=[" + _name + " d=" + target_ent.gdist_to_me + "]");
    }
    public V3 CursorToWorld {
        get {
            var conv = curs_to_world * ui.worldToGridScale;
            var h = ui.Get_H_from_gp(conv);
            return new V3(curs_to_world.X, curs_to_world.Y, h);
        }
    }

    void SameTest() {        
        ui.AddToLog("cam_cursor =[" + CursorToWorld.ToIntString() + "]");
        ui.AddToLog("me pos =[" + me_pos.ToIntString() + "]");
        ui.AddToLog("tgr ent id =[" + target_ent?.id + "]");
    }


    public Entity target_ent { get; private set; }
    Matrix4x4 worldToScreenMatrix;
    /// <summary>
    ///     Converts the World position to Screen location.
    /// </summary>
    /// <param name="worldPosition">3D world position of the entity.</param>
    /// <returns>screen location of the entity.</returns>
    public V2 WorldToScreen(V3 worldPosition) {
        var result = V2.Zero;

        Vector4 temp0 = new(worldPosition.X, worldPosition.Y, worldPosition.Z, 1.0f);
        temp0 = Vector4.Transform(temp0, worldToScreenMatrix);
        temp0 /= temp0.W;
        result.X = (temp0.X + 1.0f) * (ui.game_window_rect.Width / 2.0f);
        result.Y = (1.0f - temp0.Y) * (ui.game_window_rect.Height / 2.0f);
        return result;
    }
    public int Width { get; private set; }
    public int Height { get; private set; }


    protected override void CleanUpData() {
    }


    internal override void ToImGui() {
        base.ToImGui();
        if (ImGui.TreeNode("WindowToScreenMatrix")) {
            var d = worldToScreenMatrix;
            ImGui.Text($"{d.M11:0.00}\t{d.M12:0.00}\t{d.M13:0.00}\t{d.M14:0.00}");
            ImGui.Text($"{d.M21:0.00}\t{d.M22:0.00}\t{d.M23:0.00}\t{d.M24:0.00}");
            ImGui.Text($"{d.M31:0.00}\t{d.M32:0.00}\t{d.M33:0.00}\t{d.M34:0.00}");
            ImGui.Text($"{d.M41:0.00}\t{d.M42:0.00}\t{d.M43:0.00}\t{d.M44:0.00}");
            ImGui.TreePop();
        }
    }

}
