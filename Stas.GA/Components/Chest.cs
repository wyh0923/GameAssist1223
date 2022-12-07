using System;
using ImGuiNET;


namespace Stas.GA;

/// <summary>
///     The <see cref="Chest" /> component in the entity.
/// </summary>
public class Chest : EntComp {
    public Chest(IntPtr address) : base(address) { 
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<ChestOffsets>(this.Address);
        IsOpened = data.IsOpened;
        IsLocked = data.IsLocked;
        Quality = data.Quality;
        var dataInternal = ui.m.Read<ChestsStructInternal>(data.ChestsDataPtr);
        IsStrongbox = dataInternal.StrongboxDatPtr != IntPtr.Zero;
        IsLarge = dataInternal.IsLarge;
        OpenOnDamage = dataInternal.OpenOnDamage;
        OpenChestWhenDemonsDie = dataInternal.OpenChestWhenDemonsDie;
        DropSlots = dataInternal.DropSlots;
    }
    public int DropSlots { get; private set; }
    public bool OpenChestWhenDemonsDie { get; private set; }
    public bool OpenOnDamage { get; private set; }
    public byte Quality { get; private set; }
    public bool IsLocked { get; private set; }
    /// <summary>
    ///     Gets a value indicating whether chest is opened or not.
    /// </summary>
    public bool IsOpened { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether chest is a strongbox or not.
    /// </summary>
    public bool IsStrongbox { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether chest label is visible or not
    ///     NOTE: Breach chests, Legion chests, Normal Chests labels are visible.
    /// </summary>
    public bool IsLarge { get; private set; }

    /// <summary>
    ///     Converts the <see cref="Chest" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"IsOpened: {this.IsOpened}");
        ImGui.Text($"IsStrongbox: {this.IsStrongbox}");
        ImGui.Text($"IsLabelVisible: {this.IsLarge}");
    }

   

   
   
}