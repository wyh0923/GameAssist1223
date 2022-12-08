using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Stas.GA;
public partial class ui {
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "Start")]
    public static extern int Start(int pid, bool debug);
    static readonly Dictionary<string, MapIconsIndex> Icons;
    public static ConcurrentDictionary<IntPtr, Element> elements = new();
    public static ConcurrentDictionary<IntPtr, string> texts = new();
    public static ConcurrentDictionary<IntPtr, DateTime> w8ting_click_until = new();
    public static ConcurrentDictionary<StdWString, string> std_wstrings = new();
    public static MapIconsIndex IconIndexByName(string name) {
        name = name.Replace(" ", "").Replace("'", "");
        Icons.TryGetValue(name, out var result);
        return result;
    }

    static Thread frame_thread;
    public static Dictionary<string, string> gui_offs_nams = new();
    internal static Memory m { get; private set; }
    public static int w8 { get; } = 16;////1000 / 60 = 16(60 frame sec)
    static ui() {
        Icons = new Dictionary<string, MapIconsIndex>(200);
        foreach (var icon in Enum.GetValues(typeof(MapIconsIndex))) {
            Icons[icon.ToString()] = (MapIconsIndex)icon;
        }
        var t = typeof(guiOffset);
        foreach (var m in t.GetFields()) {
            if (!m.IsPublic)
                continue;
            var name = m.Name;
            var offset = Marshal.OffsetOf(t, name);
            gui_offs_nams["0x" + offset.ToString("X")] = name;
        }
        int w8_err = 300;
        sett = new Settings().Load<Settings>();
        udp_sound = new UdpSound();
        alert = new PreloadAlert();
        StartGameWatcher();
        SetRole();
        looter = new Looter();
        need_upd_per_frame = new List<RemoteObjectBase>() {   }; //camera//gui
        var game_not_loadin = 0;

        frame_thread = new Thread(() => {
            while (ui.b_running) {
                frame_count += 1;
                if (game_ptr == IntPtr.Zero) {
                    game_not_loadin += 1;
                    if (game_not_loadin > 1000) {
                        AddToLog("w8 game not loading... ", MessType.Critical);
                        AddToLog("I recommend that you click on \"Quit\",", MessType.Critical);
                        AddToLog("enter into the game and then start GA again", MessType.Critical);
                        AddToLog("Use [Control]+[Alt] to activate this window", MessType.Critical);
                    }
                    Thread.Sleep(200);
                    continue;
                }
               
                if (states.b_ready)
                    states.Tick(states.Address, "frame thread");

                if (curr_state == GameStateTypes.InGameState) {
                    foreach (var n in need_upd_per_frame)
                        n?.Tick(n.Address, "frame thread");
                    CheckWorker();
                    if (worker == null) {
                        ui.AddToLog("Frame err: worker need be setup", MessType.Critical);
                        continue;
                    }
                    CheckFlasks(false);
                    CheckMapPlayers();
                }

                #region tick timer & w8ting for relax CPU
                var d_elaps = sw.Elapsed.TotalMilliseconds;
                elapsed.Add(d_elaps);
                if (elapsed.Count > 60)
                    elapsed.RemoveAt(0);
                var frame_time = elapsed.Sum() / elapsed.Count;
                if (frame_time < w8) {
                    Thread.Sleep(w8 - (int)frame_time);
                }
                else {
                    Thread.Sleep(1);
                    AddToLog("Main: Big Tick Time", MessType.Error);
                }
                #endregion
            }
        });
        frame_thread.IsBackground = true;
        frame_thread.Start();
    }
   
    public static void Dispose() {
        CloseGame();
        try {
            b_running= false;
            Thread.Sleep(w8 * 5);
            frame_thread.Abort();
            watcher_thread.Abort();
            choise_thread.Abort();
            tasker.Dispose();
            nav.Dispose();
            looter.Dispose();
            worker = null;
            gui.Dispose();
        }
        catch (Exception ex) {
        }
    }
}


