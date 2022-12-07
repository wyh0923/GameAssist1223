using System.Numerics;
using ImGuiNET;

namespace Stas.GA;
/// <summary>
///  [3] ui.states.ingame_state.curr_world_data  
/// </summary>
public class WorldData : RemoteObjectBase {
    internal WorldData(IntPtr address) : base(address) {
    }
    //OnPerFrame(), "[AreaInstance] Update World Data", int.MaxValue - 3
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<WorldDataOffset>(Address);
        camera.Tick(Address + 0xA8);
        var areaInfo = ui.m.Read<WorldAreaDetailsStruct>(data.WorldAreaDetailsPtr);
        world_data.Tick(areaInfo.WorldAreaDetailsRowPtr);
    }
   

    /// <summary>
    ///     Gets the Area Details.
    /// </summary>
    public WorldAreaDat world_data { get; } = new(IntPtr.Zero);
    public Camera camera { get; } = new Camera();
  protected override void CleanUpData() {
        world_data.Tick(IntPtr.Zero);
    }


  
}