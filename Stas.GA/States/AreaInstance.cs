using System.Collections.Concurrent;
using System.Diagnostics;
using ImGuiNET;
using static System.Runtime.InteropServices.JavaScript.JSType;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;
/// <summary>
///   [3] core.states.ingame_state.curr_area_instance  =>mapper
/// </summary>
public partial class AreaInstance : RemoteObjectBase {
    #region init
    internal AreaInstance(IntPtr address) : base(address) {
       environments = new();
       EntityCaches = new() {
            new("Breach", 1104, 1108, this.AwakeEntities),
            new("LeagueAffliction", 1114, 1114, this.AwakeEntities),
            new("Hellscape", 1244, 1255, this.AwakeEntities)
        };
    }
    #endregion
   
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var data = ui.m.Read<AreaInstanceOffsets>(Address);
        //AwakeEntities.Clear();
        //EntityCaches.ForEach((e) => e.Clear());
        //need_check.Clear();

        MonsterLevel = data.MonsterLevel;
        AreaHash = data.CurrentAreaHash;

        player.Tick(data.LocalPlayerPtr);
        Debug.Assert(player.GetComp<Render>(out var rend) && rend != null);
        if (player.gpos != V2.Zero) {
            lock (me_pos) {
                if (ui.sett.max_player_debug_pos > 0) {
                    me_pos.Add(player.gpos);
                }
                if (me_pos.Count > ui.sett.max_player_debug_pos) {
                    me_pos.RemoveAt(0);
                }
            }
        }
        UpdateEnvironmentAndCaches(data.Environments);
        server_data.Tick(data.ServerDataPtr);
        UpdateEntities(data.AwakeEntities);
    }
    public void UpdateMapDate() {
        var sw = new Stopwatch();
        sw.Restart();
        var data = ui.m.Read<AreaInstanceOffsets>(Address);
        TerrainMetadata = data.TerrainMetadata;
        GridWalkableData = ui.m.ReadStdVector<byte>(TerrainMetadata.GridWalkableData);
        GridHeightData = GetTerrainHeight();
       // ui.AddToLog("GetTerrainHeight =[" + sw.Elapsed.TotalMilliseconds.ToRoundStr(0) + "]", MessType.Warning);//230

        sw.Restart();
        TgtTilesLocations = GetTgtFileData();
       // ui.AddToLog("GetTgtFileData =[" + sw.Elapsed.TotalMilliseconds.ToRoundStr(0) + "]", MessType.Warning);//24
    }
    protected override void CleanUpData() {
      
    }
   
    string entityIdFilter = string.Empty;
    string entityPathFilter = string.Empty;
    bool filterByPath = false;
    StdVector environmentPtr  = default;
    readonly List<int> environments;



    /// <summary>
    ///     Gets the Monster Level of current Area.
    /// </summary>
    public int MonsterLevel { get; private set; } = 0;

    /// <summary>
    ///     Gets the Hash of the current Area/Zone.
    ///     This value is sent to the client from the server.
    /// </summary>
    public uint AreaHash { get; private set; } = 0;

    /// <summary>
    ///     Gets the data related to the player the user is playing.
    /// </summary>
    public ServerData server_data { get; } = new(IntPtr.Zero);

    /// <summary>
    ///     Gets the player Entity.
    /// </summary>
    public Entity player { get; }  = new ();

    /// <summary>
    ///     Gets the Awake Entities of the current Area/Zone.
    ///     Awake Entities are the ones which player can interact with
    ///     e.g. Monsters, Players, NPC, Chests and etc. Sleeping entities
    ///     are opposite of awake entities e.g. Decorations, Effects, particles and etc.
    /// </summary>
    public ConcurrentDictionary<EntityNodeKey, Entity> AwakeEntities { get; } = new();

    /// <summary>
    ///     Gets important environments entity caches. This only contain awake entities.
    /// </summary>
    public List<DisappearingEntity> EntityCaches { get; }


    /// <summary>
    ///     Gets the terrain metadata data of the current Area/Zone instance.
    /// </summary>
    public TerrainStruct TerrainMetadata { get; private set; } = default;

    /// <summary>
    ///     Gets the terrain height data.
    /// </summary>
    public float[][] GridHeightData { get; private set; }

    /// <summary>
    ///     Gets the terrain data of the current Area/Zone instance.
    /// </summary>
    public byte[] GridWalkableData { get; private set; } = Array.Empty<byte>();

    /// <summary>
    ///     Gets the Disctionary of Lists containing only the named tgt tiles locations.
    /// </summary>
    public Dictionary<string, List<V2>> TgtTilesLocations { get; private set; } = new();

    /// <summary>
    ///    Gets the current zoom value of the world.
    /// </summary>
    public float Zoom {
        get {
            if (player.GetComp(out Render render)) {
                var wp = render.WorldPosition;
                var p0 = ui.camera.WorldToScreen(wp);
                wp.Z += render.ModelBounds.Z;
                var p1 = ui.camera.WorldToScreen(wp);

                return Math.Abs(p1.Y - p0.Y) / render.ModelBounds.Z;
            }
            return 0;
        }
    }
   
    private void UpdateEnvironmentAndCaches(StdVector environments) {
        this.environments.Clear();
        this.environmentPtr = environments;
        var envData = ui.m.ReadStdVector<EnvironmentStruct>(environments);
        for (var i = 0; i < envData.Length; i++) {
            this.environments.Add(envData[i].Key);
        }
        this.EntityCaches.ForEach((eCache) => eCache.UpdateState(this.environments));
    }
   
    void AddToCacheParallel(EntityNodeKey key, string path) {
        for (var i = 0; i < this.EntityCaches.Count; i++) {
            if (this.EntityCaches[i].TryAddParallel(key, path)) {
                break;
            }
        }
    }
    float[][] GetTerrainHeight() {
        var bad_ptr = 0;
        var rotationHelper = ui.RotationSelector.Values;
        var rotatorMetrixHelper = ui.RotatorHelper.Values;
        var tileData = ui.m.ReadStdVector<TileStructure>(this.TerrainMetadata.TileDetailsPtr);
        var tileHeightCache = new ConcurrentDictionary<IntPtr, sbyte[]>();
        if (tileData.Length == 0) { //may occur when changing locations during debugging
            ui.AddToLog(tName + "GetTerrainHeight err: tileData Length==0", MessType.Error);
            return default;
        }
        //for (int i = 0; i < tileData.Length; i++) {//for debug only
          
        //}
        Parallel.For(0, tileData.Length, i => {
            var val = tileData[i];
            tileHeightCache.AddOrUpdate(val.SubTileDetailsPtr,
                addr => {
                    if (addr == IntPtr.Zero) {
                        bad_ptr += 1;
                    }
                    var subTileData = ui.m.Read<SubTileStruct>(addr);
                    var subTileHeightData = ui.m.ReadStdVector<sbyte>(subTileData.SubTileHeight);
                    if (subTileHeightData.Length > TileStructure.TileToGridConversion * TileStructure.TileToGridConversion) {
                        ui.AddToLog($"found new length [" + subTileHeightData.Length + "] ");
                    }
                    return subTileHeightData;
                }, (addr, data) => data);
        });

        var gridSizeX = (int)this.TerrainMetadata.TotalTiles.X * TileStructure.TileToGridConversion;
        var gridSizeY = (int)this.TerrainMetadata.TotalTiles.Y * TileStructure.TileToGridConversion;
        Debug.Assert(gridSizeX > 0 && gridSizeY > 0 && gridSizeX < 10000 && gridSizeY < 10000);
        var result = new float[gridSizeY][];
        Parallel.For(0, gridSizeY, y => {
            result[y] = new float[gridSizeX];
            for (var x = 0; x < gridSizeX; x++) {
                var tileDataIndex = y / TileStructure.TileToGridConversion *
                    (int)this.TerrainMetadata.TotalTiles.X + x / TileStructure.TileToGridConversion;
                var mytiledata = tileData[tileDataIndex];
                var mytileHeight = tileHeightCache[mytiledata.SubTileDetailsPtr];
                var exactHeight = 0;
                if (mytileHeight.Length > 0) {
                    var gridXremaining = x % TileStructure.TileToGridConversion;
                    var gridYremaining = y % TileStructure.TileToGridConversion;
                    var tmp = TileStructure.TileToGridConversion - 1;
                    var rotatorMetrix = new int[4]
                    {
                        tmp - gridXremaining,
                        gridXremaining,
                        tmp - gridYremaining,
                        gridYremaining
                    };

                    var rotationSelected = rotationHelper[mytiledata.RotationSelector] * 3;
                    int rotatedX0 = rotatorMetrixHelper[rotationSelected];
                    int rotatedX1 = rotatorMetrixHelper[rotationSelected + 1];
                    int rotatedY0 = rotatorMetrixHelper[rotationSelected + 2];
                    var rotatedY1 = 0;
                    if (rotatedX0 == 0) {
                        rotatedY1 = 2;
                    }

                    var finalRotatedX = rotatorMetrix[rotatedX0 * 2 + rotatedX1];
                    var finalRotatedY = rotatorMetrix[rotatedY0 + rotatedY1];
                    var mytileHeightIndex = finalRotatedY * TileStructure.TileToGridConversion + finalRotatedX;
                    exactHeight = mytileHeight[mytileHeightIndex];
                }

                result[y][x] = mytiledata.TileHeight * (float)this.TerrainMetadata.TileHeightMultiplier + exactHeight;
                result[y][x] = result[y][x] * TerrainStruct.TileHeightFinalMultiplier * -1;
            }
        });

        return result;
    }

    void EntitiesWidget(string label, ConcurrentDictionary<EntityNodeKey, Entity> data) {
        if (ImGui.TreeNode($"{label} Entities ({data.Count})###${label} Entities")) {
            if (ImGui.RadioButton("Filter by Id           ", this.filterByPath == false)) {
                this.filterByPath = false;
                this.entityPathFilter = string.Empty;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Filter by Path", this.filterByPath)) {
                this.filterByPath = true;
                this.entityIdFilter = string.Empty;
            }

            if (this.filterByPath) {
                ImGui.InputText(
                    "Entity Path Filter",
                    ref this.entityPathFilter,
                    100);
            }
            else {
                ImGui.InputText(
                    "Entity Id Filter",
                    ref this.entityIdFilter,
                    10,
                    ImGuiInputTextFlags.CharsDecimal);
            }

            foreach (var entity in data) {
                if (!(string.IsNullOrEmpty(this.entityIdFilter) ||
                      $"{entity.Key.id}".Contains(this.entityIdFilter))) {
                    continue;
                }

                if (!(string.IsNullOrEmpty(this.entityPathFilter) ||
                      entity.Value.Path.ToLower().Contains(this.entityPathFilter.ToLower()))) {
                    continue;
                }

                if (ImGui.TreeNode($"{entity.Value.id} {entity.Value.Path}")) {
                    entity.Value.ToImGui();
                    ImGui.TreePop();
                }

                if (entity.Value.IsValid &&
                    entity.Value.GetComp<Render>(out var eRender)) {
                    ImGuiExt.DrawText(
                        eRender.WorldPosition,
                        $"ID: {entity.Key.id}");
                }
            }

            ImGui.TreePop();
        }
    }
  
    internal override void ToImGui() {
        base.ToImGui();
        if (ImGui.TreeNode("Environment Info")) {
            ImGuiExt.IntPtrToImGui("Address", this.environmentPtr.First);
            if (ImGui.TreeNode($"All Environments ({this.environments.Count})###AllEnvironments")) {
                for (var i = 0; i < this.environments.Count; i++) {
                    if (ImGui.Selectable($"{this.environments[i]}")) {
                        ImGui.SetClipboardText($"{this.environments[i]}");
                    }
                }

                ImGui.TreePop();
            }

            foreach (var eCache in this.EntityCaches) {
                eCache.ToImGui();
            }

            ImGui.TreePop();
        }

        ImGui.Text($"Area Hash: {this.AreaHash}");
        ImGui.Text($"Monster Level: {this.MonsterLevel}");
        ImGui.Text($"World Zoom: {this.Zoom}");
        if (ImGui.TreeNode("Terrain Metadata")) {
            ImGui.Text($"Total Tiles: {this.TerrainMetadata.TotalTiles}");
            ImGui.Text($"Tiles Data Pointer: {this.TerrainMetadata.TileDetailsPtr}");
            ImGui.Text($"Tiles Height Multiplier: {this.TerrainMetadata.TileHeightMultiplier}");
            ImGui.Text($"Grid Walkable Data: {this.TerrainMetadata.GridWalkableData}");
            ImGui.Text($"Grid Landscape Data: {this.TerrainMetadata.GridLandscapeData}");
            ImGui.Text($"Data Bytes Per Row (for Walkable/Landscape Data): {this.TerrainMetadata.BytesPerRow}");
            ImGui.TreePop();
        }

        if (this.player.GetComp<Render>(out var pPos)) {
            var y = (int)pPos.GridPosition.Y;
            var x = (int)pPos.GridPosition.X;
            if (y < this.GridHeightData.Length) {
                if (x < this.GridHeightData[0].Length) {
                    ImGui.Text("Player Pos to Terrain Height: " +
                               $"{this.GridHeightData[y][x]}");
                }
            }
        }

        this.EntitiesWidget("Awake", this.AwakeEntities);
    }
}