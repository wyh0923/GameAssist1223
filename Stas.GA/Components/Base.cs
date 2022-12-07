using System.Collections.Concurrent;
using ImGuiNET;
namespace Stas.GA;

/// <summary>
///     The <see cref="Base" /> component in the entity.
/// </summary>
public class Base : EntComp {
     public Base(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from = null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<BaseOffsets>(this.Address);
        if (BaseItemTypeDatCache.TryGetValue(data.BaseInternalPtr, out var itemName)) {
            this.ItemBaseName = itemName;
        }
        else {
            var baseItemTypeDatRow = ui.m.Read<BaseItemTypesDatOffsets>(data.BaseInternalPtr);
            var name = ui.m.ReadStdWString(baseItemTypeDatRow.BaseNamePtr);
            if (!string.IsNullOrEmpty(name)) {
                BaseItemTypeDatCache[data.BaseInternalPtr] = name;
                this.ItemBaseName = name;
            }
        }
    }
    /// <summary>
    ///     Cache the BaseItemType.Dat data to save few reads per frame.
    /// </summary>
    private static readonly ConcurrentDictionary<IntPtr, string> BaseItemTypeDatCache = new();

    /// <summary>
    ///     Gets the items base name.
    /// </summary>
    public string ItemBaseName { get; private set; } = string.Empty;

    /// <inheritdoc />
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Base Name: {this.ItemBaseName}");
    }

   

    

    private static void OnGameClose() {
        BaseItemTypeDatCache.Clear();
    }
}