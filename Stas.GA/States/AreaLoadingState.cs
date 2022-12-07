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
    bool map_must_upd = false;
    /// <summary>
    /// area loaded counter
    /// </summary>
    bool alc_must_upd = false;
    bool clf_must_uod = false;
    uint last_map_hash;
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<AreaLoadingStateOffset>(Address);
        var gst = (GameStateTypes)ui.m.Read<byte>(Address + 0x0B);
        IsLoading = data.IsLoading == 0x01;
        
        if (data.CurrentAreaName.Buffer != IntPtr.Zero && !IsLoading &&
            data.TotalLoadingScreenTimeMs > lastCache.TotalLoadingScreenTimeMs) {
            //area changed event here
            //save all static items with loot, visited&importend cells, Quest
            last_map_hash = ui.curr_map_hash;//for testonly
            ui.nav.SaveVisited();
            ui.SaveQuest();

            ui.sett.map_scale = ui.sett.map_scale_def;
            CurrentAreaName = ui.m.ReadStdWString(data.CurrentAreaName);

            lastCache = data;
            map_must_upd = true;
            alc_must_upd = true;
            clf_must_uod = true;
        }
        var gs_ok = ui.curr_state == GameStateTypes.InGameState;
        if (alc_must_upd && ui.area_change_counter.Address != default) {
            ui.area_change_counter.Tick(ui.area_change_counter.Address);
            alc_must_upd = false;
        }
        if (clf_must_uod && ui.curr_loaded_files.Address != default) {
            ui.curr_loaded_files.Tick(ui.curr_loaded_files.Address, "area was changed");
            clf_must_uod = false;
        }
        if (map_must_upd && gs_ok) {//w8 ting right game_state
            ui.curr_map.UpdateMap();
            map_must_upd = false;
        }
    }


    /// <summary>
    ///     Gets the game current Area Name.
    /// </summary>
    public string CurrentAreaName { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets a value indicating whether the game is in loading screen or not.
    /// </summary>
    internal bool IsLoading { get; private set; }

   
    /// <inheritdoc />
    protected override void CleanUpData() {
        this.CurrentAreaName = string.Empty;
    }
    /// <summary>
    ///     Converts the <see cref="AreaLoadingState" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Current Area Name: {this.CurrentAreaName}");
        ImGui.Text($"Is Loading Screen: {this.IsLoading}");
    }
}