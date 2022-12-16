using System.Diagnostics;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stas.GA;

/// <summary>
///    [2] ui.states.ingame_state 
/// </summary>
public class InGameState : RemoteObjectBase {
    internal InGameState(IntPtr address) : base(address) {
     
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero) {
            Clear();
            return;
        }
        var data = ui.m.Read<InGameStateOffset>(Address);
        //for debug info only
        var gst = (gState)ui.m.Read<byte>(Address + 0x0B);
        world_data.Tick(data.WorldData);
        area_instance.Tick(data.AreaInstanceData);
        UIHover.Tick(data.UIHover, tName);
        UiRoot.Tick(data.UiRootPtr, tName);
        gui ??= new GameUiElements();
        gui.Tick(data.IngameUi, tName);
    }
    protected override void Clear() {
        //TODO debug where and when it is called from!
        world_data.Tick(IntPtr.Zero);
        area_instance.Tick(IntPtr.Zero);
        UiRoot.Tick(IntPtr.Zero, tName + ".CleanUpData");
        gui?.Tick(IntPtr.Zero, tName + ".CleanUpData");
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
    public AreaInstance area_instance { get; } = new(default);
    /// <summary>
    ///     Gets the data related to the root ui element.
    ///     Not working for login/choise hero states
    /// </summary>
    internal Element UiRoot { get; private set; } = new Element("UiRoot");
    /// <summary>
    ///     Gets the UiRoot main child which contains all the UiElements of the game.
    /// </summary>
    public GameUiElements gui { get; private set; } = new GameUiElements();

}