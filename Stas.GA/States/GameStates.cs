using ImGuiNET;
using System.Collections.Generic;
using System.Diagnostics;

namespace Stas.GA;
/// <summary>
///     Reads and stores the global states of the game.
/// </summary>
[CodeAtt("game ctate changer")]
public class GameStates : RemoteObjectBase {
    internal GameStates(IntPtr address) : base(address) {
    }
    IntPtr currentStateAddress = IntPtr.Zero;
    GameStateStaticOffset myStaticObj;
    public bool b_ready = false;
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr; ////00007ff6f32bed30
        if (Address == IntPtr.Zero) {//from game.watcher close
            b_ready = false;
            return;
        }
        if (!b_ready) {//init analog
            myStaticObj = ui.m.Read<GameStateStaticOffset>(Address);
            var data = ui.m.Read<GameStateOffset>(myStaticObj.GameState);
            AllStates[data.State0] = 0;
            AllStates[data.State1] = (GameStateTypes)1;
            AllStates[data.State2] = (GameStateTypes)2;
            AllStates[data.State3] = (GameStateTypes)3;
            AllStates[data.State4] = (GameStateTypes)4;
            AllStates[data.State5] = (GameStateTypes)5;
            AllStates[data.State6] = (GameStateTypes)6;
            AllStates[data.State7] = (GameStateTypes)7;
            AllStates[data.State8] = (GameStateTypes)8;
            AllStates[data.State9] = (GameStateTypes)9;
            AllStates[data.State10] = (GameStateTypes)10;
            AllStates[data.State11] = (GameStateTypes)11;
            Debug.Assert(data.State0 != default && data.State4 != default);
            area_loading_state.Tick(data.State0, tName); //-1
            ingame_state.Tick(data.State4, tName); //=2
            b_ready = true;
        }
        else {
            area_loading_state.Tick(area_loading_state.Address, from);
            ingame_state.Tick(ingame_state.Address, from);
        }
        var tik_game_state = ui.m.Read<IntPtr>(ui.m.Read<StdVector>(ui.m.Read<IntPtr>(Address) + 8).Last - 0x10);// Get 2nd-last ptr.
        if (tik_game_state != currentStateAddress) {
            //here game state cahnge event
            currentStateAddress = tik_game_state;
            curr_game_state = AllStates[currentStateAddress];
            ui.AddToLog("GameStates was changed to" + _cgs, MessType.Warning);
        }
    }
    protected override void CleanUpData() {
        b_ready = false;
        myStaticObj = default;
        currentStateAddress = IntPtr.Zero;
        curr_game_state = GameStateTypes.GameNotLoaded;
        AllStates.Clear();
        area_loading_state.Tick(IntPtr.Zero);
        ingame_state.Tick(IntPtr.Zero);
    }
    /// <summary>
    ///     Gets a dictionary containing all the Game States addresses.
    /// </summary>
    public Dictionary<IntPtr, GameStateTypes> AllStates { get; } = new();
    /// <summary>
    /// [1] ui.states.area_loading_state 
    /// </summary>
    public AreaLoadingState area_loading_state { get; } = new(IntPtr.Zero);
    /// <summary>
    ///  [2] ui.states.ingame_state 
    /// </summary>
    public InGameState ingame_state { get; } = new(IntPtr.Zero);
    GameStateTypes _cgs = GameStateTypes.GameNotLoaded; //current game state
    public GameStateTypes curr_game_state {
        get => _cgs;
        private set {
            if (_cgs != value) {
                _cgs = value;
                if (value == GameStateTypes.GameNotLoaded) {
                    //debug here if client not found, error down etc
                }
                else {//current value was changed
                }
            }
        }
    }
   

    internal override void ToImGui() {
        base.ToImGui();
        if (ImGui.TreeNode("All States Info")) {
            foreach (var state in AllStates) {
                ImGuiExt.IntPtrToImGui($"{state.Value}", state.Key);
            }
            ImGui.TreePop();
        }
        ImGui.Text($"Current State: {curr_game_state}");
    }
}