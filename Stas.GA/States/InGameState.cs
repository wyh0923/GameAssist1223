﻿using System.Diagnostics;
using System.Net;
using static SharpDX.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stas.GA;

/// <summary>
///    [2] ui.states.ingame_state 
/// </summary>
public class InGameState : RemoteObjectBase {
    internal InGameState(IntPtr address) : base(address) {

    }
    public bool b_init = false;
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<InGameStateOffset>(Address);
        var gst = (GameStateTypes)ui.m.Read<byte>(Address + 0x0B);
        world_data.Tick(data.WorldData);
        area_instance.Tick(data.AreaInstanceData);
        UIHover.Tick(data.UIHover, tName);

        if (!b_init) {
            UiRoot = new Element(data.UiRootPtr, "UiRoot");//
            UiRoot.Tick(data.UiRootPtr, "test");
            gui = new GameUiElements(data.IngameUi);
            b_init = true;
        }
        else {//ones per game copy in memory and not checnge after map loading?
            //UiRootPtr can be null after map change - need test
            //Debug.Assert(UiRoot.Address == data.UiRootPtr 
            //            && gui.Address == data.IngameUi);
        }
    }
   
    protected override void CleanUpData() {
        //TODO debug where and when it is called from!
        b_init = false;
        area_instance.Tick(IntPtr.Zero);
        UiRoot.Tick(IntPtr.Zero, tName+ ".CleanUpData");
        gui.Tick(IntPtr.Zero, tName + ".CleanUpData");
        world_data.Tick(IntPtr.Zero);
    }
    /// <summary>
    /// element which is currently hovered
    /// </summary>
    public Element UIHover { get; } = new Element("UIHover");
    /// <summary>
    ///  ui.states.ingame_state.curr_world_data  [3]
    /// </summary>
    public WorldData world_data { get; } = new(IntPtr.Zero);
    /// <summary>
    ///     core.states.ingame_state.curr_area_instance[3] =>mapper
    /// </summary>
    public AreaInstance area_instance { get; } = new(IntPtr.Zero);
    /// <summary>
    ///     Gets the data related to the root ui element.
    ///     Not working for login/choise hero states
    /// </summary>
    internal Element UiRoot { get; private set; }
    /// <summary>
    ///     Gets the UiRoot main child which contains all the UiElements of the game.
    /// </summary>
    public GameUiElements gui { get; private set; }

}