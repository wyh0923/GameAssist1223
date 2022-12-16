using System.Text.RegularExpressions;
using ImGuiNET;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;
/// <summary>
///     The <see cref="Render" /> component in the entity.
/// </summary>
public class Render : EntComp {
    public Render(IntPtr address) : base(address) {
    }
  
    internal override void Tick(IntPtr ptr, string from=null) {
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<RenderOffsets>(Address);
        WorldPosition = data.CurrentWorldPosition;
        if (last_pos != V3.Zero && (last_pos.X != WorldPosition.X
         || last_pos.Y != WorldPosition.Y
          || last_pos.Z != WorldPosition.Z)) {
            b_can_move = true;
        }
        ModelBounds = data.CharactorModelBounds;
        TerrainHeight = (float)Math.Round(data.TerrainHeight, 4);
       
        gridPos2D.X = data.CurrentWorldPosition.X * ui.worldToGridScale;
        gridPos2D.Y = data.CurrentWorldPosition.Y * ui.worldToGridScale;

        if (last_name == DateTime.MinValue) {//reading only one
            var _name = data.name.GetVaue();
            if (r.IsMatch(_name))
                _name = r.Matches(_name)[0].Groups[1].Value;
            Name = _name;
            last_name = DateTime.Now;
        }
    }
    DateTime last_name = DateTime.MinValue;
    V3 last_pos;
    public bool b_can_move = false;
    Regex r = new Regex("{([^{}]+)}");
    public string Name { get; private set; }

    V2 gridPos2D;
    /// <summary>
    ///     Gets the position where entity is located on the grid (map).
    /// </summary>
    public V2 gpos_f {
        get => this.gridPos2D;
        private set => this.gridPos2D = value;
    }

    /// <summary>
    ///     Gets the position where entity is located on the grid (map).
    /// </summary>
    public StdTuple3D<float> ModelBounds { get; private set; }

    /// <summary>
    ///     Gets the postion where entity is rendered in the game world.
    ///     NOTE: Z-Axis is pointing to the (visible/invisible) healthbar.
    /// </summary>
    public V3 WorldPosition { get; private set; }

    /// <summary>
    ///     Gets the terrain height on which the Entity is standing.
    /// </summary>
    public float TerrainHeight { get; private set; }
   
    /// <summary>
    ///     Converts the <see cref="Render" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Grid Position: {this.gridPos2D}");
        ImGui.Text($"World Position: {this.WorldPosition}");
        ImGui.Text($"Terrain Height (Z-Axis): {this.TerrainHeight}");
        ImGui.Text($"Model Bounds: {this.ModelBounds}");
    }
}