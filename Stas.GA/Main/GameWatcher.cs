﻿using Stas.GA.Offsets;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
namespace Stas.GA;

public partial class ui {
    static bool closeForcefully = false;
    static Thread choise_thread, watcher_thread;
    /// <summary>
    ///     Initializes a new instance of the <see cref="PrecessWatcher" /> class.
    /// </summary>
    static void StartGameWatcher() {
        choise_thread = new Thread(() => {
            while (b_running) {
                if (!DrawMain.b_ready) {
                    AddToLog("w8 DrawMain b_ready...", MessType.Warning);
                    Thread.Sleep(w8 * 2);
                }
                var _pa = Process.GetProcessesByName(ui.sett.pp_name);
                curr_poe_count = _pa.Length;
                if (_pa.Length == 0) {
                    CloseGame();
                    Thread.Sleep(3000);
                }
                else if (_pa.Length == 1) {
                    if (game_process != null) {
                        if (game_process.Id != _pa[0].Id) {
                            CloseGame();
                            game_process = _pa[0];
                            OpenGame();
                        }
                    }
                    else {
                        game_process = _pa[0];
                        OpenGame();
                    }
                    Thread.Sleep(1000);//all ok just w8
                }
                else if (_pa.Length == 2) {
                    //TODO trader integration here
                }
                else {
                    ui.AddToLog("we have 2+ POE process", MessType.Critical);
                }
                #region CPU & Memory usage
                var memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memStatus)) {
                    mem = 100 - (float)memStatus.ullAvailPhys / memStatus.ullTotalPhys * 100;
                }

                var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                var value = 0f;
                while ((value = cpuCounter.NextValue()) == 0f) {
                    Thread.Sleep(1000 / 60);
                }
                cpus.Add(value);
                if (cpus.Count > 10)
                    cpus.RemoveAt(0);
                cpu = cpus.Sum() / cpus.Count;
                #endregion
                Thread.Sleep(10); //cpu 
            };
        });
        choise_thread.IsBackground = true;
        choise_thread.Start();

        watcher_thread = new Thread(() => {
            while (b_running) {
                // Have to check MainWindowHandle because sometime HasExited returns false even when game isn't running..
                try {
                    if (game_process != null && (game_process.HasExited || closeForcefully
                                || game_process.MainWindowHandle.ToInt64() <= 0x00)) {
                        closeForcefully = false;
                        CloseGame();
                    }
                }
                catch (Exception) {
                    closeForcefully = false;
                    CloseGame();
                }
                   
                curr_top_ptr = EXT.GetForegroundWindow();
                Thread.Sleep(100); //200ms
            }
        });
        watcher_thread.IsBackground = true;
        watcher_thread.Start();
    }

    /// <summary>
    ///     Gets the static addresses (along with their names) found in the GameProcess
    ///     based on the GameOffsets.StaticOffsets file.
    /// </summary>
    internal static Dictionary<PattNams, IntPtr> base_offsets { get; } = new();
    public static IntPtr game_ptr => b_bad_process ? IntPtr.Zero : game_process.MainWindowHandle;
    /// <summary>
    /// need for draw/set role to a process
    /// </summary>
    public static int curr_poe_count { get; private set; }
    public static float cpu { get; private set; }
    public static float mem { get; private set; }
    static List<float> cpus = new List<float>();
    public static Process game_process { get; private set; }
    public static IntPtr curr_top_ptr { get; private set; }


    public static long memory_using => game_process.PrivateMemorySize64 / (1024 * 1024);
    public static void SetCurrentGame(int pid) {
        var _pa = Process.GetProcessesByName(ui.sett.pp_name);
        Process new_p = null;
        foreach (var p in _pa) {
            if (p.Id == pid) {
                new_p = p;
                break;
            }
        }
        Debug.Assert(new_p != null);
        if (game_process != null) {
            if (game_process.Id != pid) {
                CloseGame();
            }
        }
        else {
            game_process = new_p;
            OpenGame();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX() {
            this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    extern static bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    static bool b_bad_process {
        get {
            try {
                return game_process == null || game_process.HasExited || game_process.MainWindowHandle.ToInt64() <= 0x00;
            }
            catch (Exception ex) {
                ui.AddToLog("b_bad_process err: game closed?", MessType.Critical);
                return true;
            }
        }
    }
    /// <summary>
    ///     Opens the handle for the game process.
    /// </summary>
    static bool OpenGame() {
        m = new Memory(game_process.Id);
        var b_dll_ok = false;
        if (Start(game_process.Id, true) == 0) {
            b_dll_ok = true;
        }
        else {
            ui.AddToLog("Can't start Native.Dll", MessType.Critical);
        }
        var reader_ok = m != null && !m.IsClosed && !m.IsInvalid;
        if (!reader_ok || !b_dll_ok || b_bad_process) {
            //its possible if we close game
            ui.AddToLog("OpenGame err=> same bad conditon...", MessType.Critical);
            return false;
        }
        LoadPatterns();
        return true;
    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPatt")]
    public static extern int GetPatt(ref Patt_wrapp res);
    static unsafe void LoadPatterns() {
        var sw = new SW("LoadPatterns");
        var procSize = game_process.MainModule.ModuleMemorySize;
        var baseAddress = game_process.MainModule.BaseAddress;

        sw.Restart();
        const int patt_need= 7;
        while (base_offsets.Count!= patt_need) {
            Patt_wrapp pw = default;
            if (GetPatt(ref pw) != 0) {
                AddToLog("LoadPatterns error", MessType.Critical);
                return;
            }
            using (var ums = new UnmanagedMemoryStream((byte*)pw.ptr, pw.ba_size, pw.ba_size, FileAccess.Read)) {
                var ba = new byte[pw.ba_size];
                ums.Read(ba, 0, pw.ba_size);
                using (var ms = new MemoryStream(ba)) {
                    using (var br = new BinaryReader(ms)) {
                        while (ms.Position < ms.Length) {
                            var patt = new Patt();
                            patt.Fill(br);
                            base_offsets[patt.patt] = new IntPtr(patt.ptr);
                        }
                    }
                }
            }
            if (base_offsets.Count != patt_need) {
                AddToLog("w8 correct patterns", MessType.Critical);
                Thread.Sleep(100);
            }
        }//debug here - its 
       
        states.Tick( base_offsets[ PattNams.GameStates]); 
        area_change_counter.Tick( base_offsets[PattNams.AreaChangeCounter]);
        //must be after area counter for correct "cold start" alerts on same map
        curr_loaded_files.Tick(base_offsets[PattNams.FileRoot]);
        GameScale.Tick(base_offsets[PattNams.GameWindowScaleValues]);
        RotationSelector.Tick(base_offsets[PattNams.TerrainRotationSelector]);
        RotatorHelper.Tick(base_offsets[PattNams.TerrainRotatorHelper]);
        var game_state_offs = ( base_offsets[PattNams.GameStates] - baseAddress).ToString("X");
        //sw.Print(); //this o ms if CurrentAreaLoadedFiles will load new thread
    }
   /// <summary>
   /// this not dispose - just close old one
   /// </summary>
    static void CloseGame() {
        states.Tick(IntPtr.Zero);
        curr_loaded_files.Tick(IntPtr.Zero);
        area_change_counter.Tick(IntPtr.Zero);
        GameScale.Tick(IntPtr.Zero);
        RotationSelector.Tick(IntPtr.Zero);
        RotatorHelper.Tick(IntPtr.Zero);
        m?.Dispose();
        game_process?.Close();
        tasker?.Stop("CloseGame");
    }
}