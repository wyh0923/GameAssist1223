using ImGuiNET;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Stas.GA;
/// <summary>
/// [1] ui.states.area_loading_state
/// </summary>
[CodeAtt("area changed generator")]
public sealed class AreaLoadingState : RemoteObjectBase {

    internal AreaLoadingState(IntPtr address) : base(address) {
    }
    AreaLoadingStateOffset lastCache;

    uint last_map_hash = 0; //for debug only
    internal override void Tick(IntPtr ptr, string from) {
        Address = ptr;
        if (Address == IntPtr.Zero) {
            Clear();
            return;
        }

        var data = ui.m.Read<AreaLoadingStateOffset>(Address);
        var gst = (gState)ui.m.Read<byte>(Address + 0x0B);
        IsLoading = data.IsLoading == 0x01;

        if (data.CurrentAreaName.Buffer != IntPtr.Zero && !IsLoading &&
            data.TotalLoadingScreenTimeMs > lastCache.TotalLoadingScreenTimeMs) {
            //area changed event here
            //save all static items with loot, visited&importend cells, Quest
            if (ui.curr_map_hash > 0) {
                last_map_hash = ui.curr_map_hash;
                ui.nav.SaveVisited();
                ui.SaveQuest();
            }
            lastCache = data;
            CurrentAreaName = ui.m.ReadStdWString(data.CurrentAreaName);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ui.curr_map.UpdateMap));
        }
    }
    protected override void Clear() {
        lastCache = default;
        CurrentAreaName = string.Empty;
    }
    /// <summary>
    ///     Gets the game current Area Name.
    /// </summary>
    public string CurrentAreaName { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets a value indicating whether the game is in loading screen or not.
    /// </summary>
    internal bool IsLoading { get; private set; }


    /// <summary>
    ///     Converts the <see cref="AreaLoadingState" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Current Area Name: {this.CurrentAreaName}");
        ImGui.Text($"Is Loading Screen: {this.IsLoading}");
    }
}