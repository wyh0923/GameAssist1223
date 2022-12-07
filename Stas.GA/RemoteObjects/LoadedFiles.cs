using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using ImGuiNET;
namespace Stas.GA;
/// <summary>
///     Gathers the files loaded in the game for the current area.
/// </summary>
[CodeAtt("must be updated after area changed")]
public class LoadedFiles : RemoteObjectBase {
    string searchText = string.Empty;
    string[] searchTextSplit = Array.Empty<string>();

    internal LoadedFiles(IntPtr address) : base(address) {
    }
    uint last_map_hash = 0;
    public bool b_ready { get; private set; }
    internal override void Tick(IntPtr ptr, string from=null) {
        Address = ptr;
        if (Address == IntPtr.Zero)
            return;
        var sw = new SW("LF upd+PA parce");
        b_ready = false;
        var upd_thread = new Thread(() => {
            sw.Restart();
            var bad_states = ui.states.Address == IntPtr.Zero;
            var bad_counter = ui.area_change_counter.Value == int.MaxValue; //not loaded jet
                                                                            //Debug.Assert(!bad_states && !bad_counter);
            if (ui.b_home || ui.curr_map_hash == last_map_hash) {
                return;
            }

            CleanUpData();
            last_map_hash = ui.curr_map_hash;
            var filesRootObjs = this.GetAllPointers();
            for (var i = 0; i < filesRootObjs.Length; i++) {
                this.ScanForFilesParallel(ui.m, filesRootObjs[i]);
            }
            ui.alert.AreaChange();
            sw.Print();
            b_ready = true;
        });
        upd_thread.IsBackground = true;
        upd_thread.Start();
    }
    
    /// <summary>
    ///     Gets the pathname of the files.
    /// </summary>
    public ConcurrentDictionary<string, int> PathNames { get; } = new();
    protected override void CleanUpData() {
        PathNames.Clear();
        last_map_hash = 0;
        b_ready = false;
    }

    private LoadedFilesRootObject[] GetAllPointers() {
        var totalFiles = LoadedFilesRootObject.TotalCount;
        return ui.m.ReadMemoryArray<LoadedFilesRootObject>(this.Address, totalFiles);
    }

    private void ScanForFilesParallel(Memory reader, LoadedFilesRootObject filesRootObj) {
        var filesPtr = reader.ReadStdBucket<FilesPointerStructure>(filesRootObj.LoadedFiles);
        Parallel.ForEach(filesPtr, fileNode => {
            this.AddFileIfLoadedInCurrentArea(reader, fileNode.FilesPointer);
        });
    }

    private void AddFileIfLoadedInCurrentArea(Memory reader, IntPtr address) {
        var information = reader.Read<FileInfoValueStruct>(address);
        if (information.AreaChangeCount > FileInfoValueStruct.IGNORE_FIRST_X_AREAS &&
            information.AreaChangeCount == ui.area_change_counter.Value) {
            var name = reader.ReadStdWString(information.Name).Split('@')[0];
            this.PathNames.AddOrUpdate(name, information.AreaChangeCount,
                (key, oldValue) => { return Math.Max(oldValue, information.AreaChangeCount); });
        }
    }

    /// <summary>
    ///     Converts the <see cref="LoadedFiles" /> class data to ImGui.
    /// </summary>
    internal override void ToImGui() {
        base.ToImGui();
        ImGui.Text($"Total Loaded Files in current area: {this.PathNames.Count}");
        ImGui.TextWrapped("NOTE: The Overlay caches the preloads when you enter a new map. " +
                          "This cache is only cleared & updated when you enter a new Map. Going to town or " +
                          "hideout isn't considered a new Map. So basically you can find important preloads " +
                          "even after you have completed the whole map/gone to town/hideouts and " +
                          "entered the same Map again.");
        if (!b_ready) {
            var fname = ui.curr_map_name + "[" + ui.curr_map_hash.ToString("X") + "].txt";
            ImGui.Text("File:"+ fname);
            ImGui.SameLine();
            var dir_name = "preload_dumps";
            if (ImGui.Button("Save")) {
                Directory.CreateDirectory(dir_name);
                var dataToWrite = this.PathNames.Keys.ToList();
                dataToWrite.Sort();
                File.WriteAllText(  Path.Join(dir_name, fname), string.Join("\n", dataToWrite));
            }
            ImGuiExt.ToolTip("Chek file out in dir=["+ dir_name + "]");
        }
        else {//only 600 ms here possible
            ImGuiExt.DrawDisabledButton("Save");
            ImGuiExt.ToolTip("Map not loaded well");

            if (ImGui.Button(ui.curr_map_name)) {
                ui.ReloadGameState();
            }
            ImGuiExt.ToolTip("Click for reload files");
        }

        ImGui.Text("Search:    ");
        ImGui.SameLine();
        if (ImGui.InputText("##LoadedFiles", ref this.searchText, 50)) {
            this.searchTextSplit = this.searchText.ToLower().Split(",", StringSplitOptions.RemoveEmptyEntries);
        }

        ImGui.Text("NOTE: Search is Case-Insensitive. Use commas (,) to narrow down the resulting files.");
        if (!string.IsNullOrEmpty(this.searchText)) {
            ImGui.BeginChild("Result##loadedfiles", Vector2.Zero, true);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
            foreach (var kv in this.PathNames) {
                var containsAll = true;
                for (var i = 0; i < this.searchTextSplit.Length; i++) {
                    if (!kv.Key.ToLower().Contains(this.searchTextSplit[i])) {
                        containsAll = false;
                    }
                }

                if (containsAll) {
                    if (ImGui.SmallButton($"AreaId: {kv.Value} Path: {kv.Key}")) {
                        ImGui.SetClipboardText(kv.Key);
                    }
                }
            }
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }
}