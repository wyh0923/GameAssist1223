using ImGuiNET;
namespace Stas.GA; 

/// <summary>
///     Reads the Game Window Scale values from the game.
///     It's good to read from the game because there are 6 instances of them for different
///     type of Ui-Elements. Only reads when game window moves/resize.
/// </summary>
public class GameWindowScale : RemoteObjectBase {
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameWindowScale" /> class.
    /// </summary>
    /// <param name="address">address of the remote memory object.</param>
    internal GameWindowScale(IntPtr address) : base(address) {
    }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.ReadMemoryArray<float>(this.Address, this.Values.Length);
        for (var i = 0; i < data.Length; i++) {
            this.Values[i] = data[i];
        }
    }

    protected override void Clear() {
        for (var i = 0; i < this.Values.Length; i++) {
            this.Values[i] = 1f;
        }
    }
    /// <summary>
    ///     Gets the current game window scale values.
    /// </summary>
    public float[] Values { get; } = new float[0x128];

    /// <summary>
    ///     Gets the Scale Value depending on the index and multiplier.
    /// </summary>
    /// <param name="index">index read from the Ui-Element.</param>
    /// <param name="multiplier">multiplier read from the Ui-Element.</param>
    /// <returns>Returns the Width and Height scale value valid for the specific Ui-Element.</returns>
    public (float WidthScale, float HeightScale) GetScaleValue(int index, float multiplier) {
        var widthScale = multiplier;
        var heightScale = multiplier;
        switch (index) {
            case 1:
                widthScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_1];
                heightScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_1 + 1];
                break;
            case 2:
                widthScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_2];
                heightScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_2 + 1];
                break;
            case 3:
                widthScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_3];
                heightScale *= this.Values[UiElementBaseFuncs.SCALE_INDEX_3 + 1];
                break;
        }

        return (widthScale, heightScale);
    }

   

   


    /// <summary>
    ///     Converts the <see cref="GameWindowScale" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Index 1 (0x{UiElementBaseFuncs.SCALE_INDEX_1:X}): width, height {this.GetScaleValue(1, 1)} ratio");
        ImGui.Text($"Index 2 (0x{UiElementBaseFuncs.SCALE_INDEX_2:X}): width, height {this.GetScaleValue(2, 1)} ratio");
        ImGui.Text($"Index 3 (0x{UiElementBaseFuncs.SCALE_INDEX_3:X}): width, height {this.GetScaleValue(3, 1)} ratio");
    }
}