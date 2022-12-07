using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
namespace Stas.GA; 

public delegate void InputPretector(Keys key,string from,  string trace);
public static class Keyboard {
    public static InputPretector Protect;
    [DllImport("user32.dll")]
    private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    const int KEYEVENTF_EXTENDEDKEY = 0x0001;
    const int KEYEVENTF_KEYUP = 0x0002;
    const int KEY_TOGGLED = 0x0001;
    const int KEY_PRESSED = 0x8000;
    const int ACTION_DELAY = 1;
    //const int WM_KEYUP = 0x0101;
    //const int WM_SYSKEYUP = 0x0105;

    public static void KeyUp(Keys key, string _info=null) {
        if (!ui.b_game_top) {
            ui.DebugButton(key,"try KeyUp, but POE not TOP");
            return;
        }
        keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); //0x7F
        
        if(ui.buttons.ContainsKey(key))
            ui.buttons[key].up_count += 1;
        ui.DebugButton(key, "Up ok..."+ _info);
    }

    /// <summary>
    ///it is safe. Default delay - 150 ms
    /// </summary>
    /// <param name="key"></param>
    public static void KeyPress(Keys key, string from = null) {
        if (!ui.b_game_top) {
            ui.DebugButton(key, from +"..try KeyPress, but POE not TOP");
            return;
        }
        if (key == Keys.D3) {
        }
        KeyDown(key, from);
        Thread.Sleep(ACTION_DELAY);
        KeyUp(key, from);
    }

    static double mdi = 150;  //minimal_down_interval  //ms
    public delegate void MDownInfoDelegate(string write);
    static ConcurrentDictionary<Keys, (Stopwatch, int)> using_keys =
        new ConcurrentDictionary<Keys, (Stopwatch, int)>();
    public static void KeyDown(Keys key, string from = null) {
        if (!ui.b_game_top) {
            ui.DebugButton(key, from + ".. try KeyDown, but POE not TOP", MessType.Warning);
            return;
        }
        if(!using_keys.ContainsKey(key))
            using_keys.TryAdd(key, (new Stopwatch(), 0));
        var elaps = using_keys[key].Item1.Elapsed.TotalMilliseconds;
        if(elaps < mdi) {
            Thread.Sleep((int)(mdi - elaps));
            // act?.Invoke("SafeLeftDown err: so fast="+ elaps);
            var last = using_keys[key];
            using_keys[key] = (last.Item1, last.Item2 += 1);
            if(using_keys[key].Item2 > 3) {
                if(Protect != null)
                    Protect.Invoke(key, from, Environment.StackTrace);
                else
                    ui.DebugButton(key, from +":try KeyDown to fast");
            }
            using_keys[key].Item1.Restart();
        }
        keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        if (ui.buttons.ContainsKey(key))
            ui.buttons[key].down_count += 1;
        ui.DebugButton(key,"Down ok.." + from);
    }
    [DllImport("USER32.dll")]
    private static extern short GetKeyState(int nVirtKey);
    static ConcurrentDictionary<Keys, DateTime> last_down = new ConcurrentDictionary<Keys, DateTime>();
    public static bool b_Try_press_key(Keys key, string from =null, int interv = 500, bool debug = false) {
        if ((GetKeyState((int)key) & KEY_PRESSED) != 0) {
            var last_ok = last_down.ContainsKey(key) && last_down[key].AddMilliseconds(interv) < DateTime.Now;
            if (!last_down.ContainsKey(key) || last_ok) {
                last_down[key] = DateTime.Now;
                if (debug)
                    ui.AddToLog("Press =>" + key + "<= " + from);
                return true;
            } else {
                if (debug)
                    ui.AddToLog("Press =>" + key + "<= to fast.. " + from, MessType.Warning);
                return false;
            }
        }
        return false;
    }
    public static bool IsKeyDown(Keys key, string from=null) {
        return GetKeyState((int)key) < 0;
    }

    //public static bool IsKeyUp(Keys key) {
    //    if(key == Keys.LControlKey || key == Keys.RControlKey || key==Keys.LMenu || key == Keys.RMenu)
    //        return Convert.ToBoolean(GetKeyState((int)key) & WM_SYSKEYUP);
    //    else
    //        return Convert.ToBoolean(GetKeyState((int)key) & WM_KEYUP);
    //}

    public static bool IsKeyPressed(Keys key) {
        return Convert.ToBoolean(GetKeyState((int)key) & KEY_PRESSED);
    }

    public static bool IsKeyToggled(Keys key) {
        return Convert.ToBoolean(GetKeyState((int)key) & KEY_TOGGLED);
    }
}
