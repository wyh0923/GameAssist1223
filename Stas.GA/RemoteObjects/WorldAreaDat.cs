﻿using ImGuiNET;
namespace Stas.GA; 


/// <summary>
///     Points to a row in WorldArea.dat file.
/// </summary>
public class WorldAreaDat : RemoteObjectBase {
    internal WorldAreaDat(IntPtr address)  : base(address) { 
    }
    //this never update without addres changed
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == default)
            return;
        var data = ui.m.Read<WorldAreaDatOffsets>(this.Address);
        this.Id = ui.m.ReadUnicodeString(data.IdPtr);
        this.Name = ui.m.ReadUnicodeString(data.NamePtr);
        this.IsTown = data.IsTown || this.Id == "HeistHub";
        this.HasWaypoint = data.HasWaypoint || this.Id == "HeistHub";
        this.IsHideout = this.Id.ToLower().Contains("hideout");
        this.IsBattleRoyale = this.Id.ToLower().Contains("exileroyale");
    }
    protected override void Clear() {
        this.Id = string.Empty;
        this.Name = string.Empty;
        this.Act = 0x00;
        this.IsTown = false;
        this.IsHideout = false;
        this.IsBattleRoyale = false;
        this.HasWaypoint = false;
    }
    /// <summary>
    ///     Gets the Area Id string.
    /// </summary>
    public string Id { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the Area name.
    ///     The value is same as in <see cref="AreaLoadingState.CurrentAreaName" />.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the area Act number.
    /// </summary>
    public int Act { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the area is town or not.
    /// </summary>
    public bool IsTown { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether area is hideout or not.
    /// </summary>
    public bool IsHideout { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether player is in Battle Royale or not.
    /// </summary>
    public bool IsBattleRoyale { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether area has a waypoint or not.
    /// </summary>
    public bool HasWaypoint { get; private set; }

  

   
  
    /// <summary>
    ///     Converts the <see cref="WorldAreaDat" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Id: {this.Id}");
        ImGui.Text($"Name: {this.Name}");
        ImGui.Text($"Is Town: {this.IsTown}");
        ImGui.Text($"Is Hideout: {this.IsHideout}");
        ImGui.Text($"Is BattleRoyale: {this.IsBattleRoyale}");
        ImGui.Text($"Has Waypoint: {this.HasWaypoint}");
    }
}