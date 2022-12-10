using ImGuiNET;
using System.Runtime.InteropServices;

namespace Stas.GA;


public class Life : RemoteObjectBase {
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetLifeOffsets")]
    static extern IntPtr GetLifeOffsets(IntPtr ptr, ref LifeOffset offs);
    public Life(IntPtr address)  : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;        
        var data = new LifeOffset();
        GetLifeOffsets(Address, ref data);
      
        owner_addr = data.Header.EntityPtr;
        this.Health = data.Health;
        this.EnergyShield = data.EnergyShield;
        this.Mana = data.Mana;
        this.IsAlive = data.Health.Current > 0;
    }
   
    public IntPtr owner_addr { get; private set; } = IntPtr.Zero;

    /// <summary>
    ///     Gets a value indicating whether the entity is alive or not.
    /// </summary>
    public bool IsAlive { get; private set; } = true;

    /// <summary>
    ///     Gets the health related information of the entity.
    /// </summary>
    public VitalStruct Health { get; private set; }

    /// <summary>
    ///     Gets the energyshield related information of the entity.
    /// </summary>
    public VitalStruct EnergyShield { get; private set; }

    /// <summary>
    ///     Gets the mana related information of the entity.
    /// </summary>
    public VitalStruct Mana { get; private set; }

    /// <inheritdoc />
    protected override void CleanUpData() {
        ui.AddToLog(tName + ".CleanUpData need implement", MessType.Critical);
    }



    private void VitalToImGui(VitalStruct data) {
        ImGuiExt.IntPtrToImGui("PtrToSelf", data.PtrToLifeComponent);
        ImGui.Text($"Regeneration: {data.Regeneration}");
        ImGui.Text($"Total: {data.Total}");
        ImGui.Text($"ReservedFlat: {data.ReservedFlat}");
        ImGui.Text($"Current: {data.Current}");
        ImGui.Text($"Reserved(%%): {data.ReservedPercent}");
        ImGui.Text($"Current(%%): {data.CurrentInPercent}");
    }

    /// <summary>
    ///     Converts the <see cref="Life" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();

        if (ImGui.TreeNode("Health")) {
            this.VitalToImGui(this.Health);
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Energy Shield")) {
            this.VitalToImGui(this.EnergyShield);
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Mana")) {
            this.VitalToImGui(this.Mana);
            ImGui.TreePop();
        }
    }
}