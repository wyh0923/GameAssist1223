#region using
using System.Numerics;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Point = System.Drawing.Point; 
using V2 = System.Numerics.Vector2;
using System.Text;
#endregion

namespace Stas.GA {
    public enum MouseButton {
        Left,
        Right,
        Middle,
        Extra1,
        Extra2,
    }
    public delegate void AddToLogDelegate(string str, bool b_error = false);
    public class Mouse {
        static Stopwatch sw = new Stopwatch();
        private const int KEY_TOGGLED = 0x0001;
        private const int KEY_PRESSED = 0x8000;
        public enum MouseEvents {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            Wheel = 0x800
        }
        public const int DELAY_CLICK = 5;
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out Point lpPoint);
        //static Vector2 GetCursorPosition() {
        //    var ptr = EXT.GetActiveWindow();
        //    GetCursorPos(out var rpoint);
        //    ScreenToClient(ptr, ref rpoint); //add GetActiveWindowRectangle() Left & Top to result = dont need at all
        //    return new Vector2(rpoint.X, rpoint.Y);
        //}
        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
        public static bool TryRightClick(string _from, int _cd = 500) {
            if (RightDown(_from, _cd)) {
                Thread.Sleep(30);
                RightUp();
                return true;
            }
            return false;
        }
        static Stopwatch sw_right_down = new Stopwatch();
        static bool RightDown(string _from, int _cd = 500) {
            var elaps = sw_right_down.Elapsed.TotalMilliseconds;
            if (elaps < _cd) {
                ui.AddToLog(_from + " Try RightDown to fast", MessType.Error);
                return false;
            }
            mouse_event((int)MouseEvents.RightDown, 0, 0, 0, 0);
            sw_right_down.Restart();
            return true;
        }
        public static V2 GetCursorPosition() {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            return new V2(lpPoint.X, lpPoint.Y);
        }
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);
        public static bool HotKeyPressed(Keys key, int interv = 400, bool debug = true) {
            if((GetKeyState((int)key) & KEY_PRESSED) != 0 ) {
                if(!down_time.ContainsKey(key) ||  down_time[key].AddMilliseconds(interv) < DateTime.Now) {
                    down_time[key] = DateTime.Now;
                    if(debug)
                        ui.AddToLog("HotKey " + key.ToString() + " used OK");
                    return true;
                }
                else {
                    if(debug )
                        ui.AddToLog("HotKey " + key.ToString() + " was already pressed recently");
                    return false;
                }
            }
            return false;
        }

        static ConcurrentDictionary<Keys, (Stopwatch, int)> using_keys =
            new ConcurrentDictionary<Keys, (Stopwatch, int)>();
        static ConcurrentDictionary<Keys, DateTime> down_time = new ConcurrentDictionary<Keys, DateTime>();
        //TODO here bug UP sametime for same mouse buttons => not detecting 
        public static bool IsButtonDown(Keys key) {
            return GetKeyState((int)key) < 0;
        }
     

        public static void RightUp() {
            mouse_event((int)MouseEvents.RightUp, 0, 0, 0, 0);
            Thread.Sleep(DELAY_CLICK);
        }

        public static void VerticalScroll(bool forward, int clicks=1) {
            if(forward)
                mouse_event((int)MouseEvents.Wheel, 0, 0, clicks * 120, 0);
            else
                mouse_event((int)MouseEvents.Wheel, 0, 0, -(clicks * 120), 0);
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool block);

        public static void LeftClick(string _from) {
            LeftDown(_from);
            LeftUp(_from);
        }
        public static DateTime last_left_down;
        public delegate void MDownInfoDelegate(string write);
        public const int min_down_interval = 300; //ms
        public static void LeftDown(string from, int _mdi= min_down_interval, bool debug = false) { //minimal_down_interval  //ms
            if (last_left_down.AddMilliseconds(_mdi) < DateTime.Now) {
                Thread.Sleep(_mdi);
                ui.AddToLog(from +" Try LeftDown to fast", MessType.Error);
            }
            mouse_event((int)MouseEvents.LeftDown, 0, 0, 0, 0);
            last_left_down = DateTime.Now;
            left_down_count += 1;
            last_set_id = 0;
            if (debug) {
                ui.AddToLog("LeftDown.." + from);
                ui.AddToLog("LeftDown..[" + left_down_count + "]");
            }
        }
      
        public static void LeftUp(string from, bool debug = false) {
            if (IsButtonDown(Keys.LButton)) {
                //if (!from.Contains("StuckStop")) { 
                //}
                mouse_event((int)MouseEvents.LeftUp, 0, 0, 0, 0);
                if(debug)
                    ui.AddToLog(from + " LeftUp OK");
            }
        }

        public static void blockInput(bool block) {
            BlockInput(block);
        }

        static StringBuilder sb_cond = new();
        public static DateTime last_setted;//last auto mouse setted time
                                           //if u not full screen/have screen scale factor - correct screen point [SP] beffor call here</param>
        public static void Reset() {
            spa.Clear();
            last_set_id = 0;
            left_down_count = 0;
        }
        public static int left_down_count;
        public static int last_set_id;
        /// <summary>
        /// this is a budget implementation of mouse movement - I strongly recommend that you do not use this for trade bots
        /// </summary>
        public static void SetCursor(V2 tp, string info, int step = 5,
                bool stop_moving = true, bool must_be_safe = true, int w8 = 0) {
            if (ui.b_alt) {
                ui.AddToLog("SetCursor... alt was pressed", MessType.Error);
                return;
            }
            if (w8 > 0 && last_setted.AddMilliseconds(w8) > DateTime.Now) {
                return;//debug here for get call from
            }
            var ccp = GetCursorPosition();//current cursor position
            var debug = true;

            var dx = Math.Round(Math.Abs(tp.X - ccp.X), 0);
            var dy = Math.Round(Math.Abs(tp.Y - ccp.Y), 0);
            if (dx < 2 && dy < 2) {
                if (Math.Round(tp.Y, 0) == 38) {
                }
                ui.AddToLog("SetCursor... low start delta", MessType.Warning);
                //ui.AddToLog("SetCursor...["+ tp .ToIntString()+ "] low start delta", MessType.Warning);
                return;
            }
            bool cond_err(out string res) {
                sb_cond.Clear();
                if (ui.b_alt)
                    sb_cond.Append("b_alt ");
                if (Keyboard.IsKeyDown(ui.worker.main.key))
                    sb_cond.Append("worker.main.key ");
                if (!ui.b_game_top)
                    sb_cond.Append("b_top");
                if (ui.test_elem != null)
                    sb_cond.Append("test_elem != null");
                if (!tp.PointInRectangle(ui.game_window_rect))
                    sb_cond.Append("TP out of game window");
                if (must_be_safe && !ui.b_sp_is_safe(tp))
                    sb_cond.Append("TP outside the safe zone");
                res = sb_cond.ToString();
                return sb_cond.Length > 0;
            }

            if (cond_err(out var ce)) {
                if (ce == "TP outside the safe zone") {
                }
                ui.AddToLog("SetCursor.." + info + ".. err=" + ce, MessType.Error);
                return;
            }

            if (ui.worker != null && stop_moving) {
                ui.worker.StopMoving("SetCursor.. " + info, true);
            }
            if (step > 0) {
                sw.Restart();
                float dist = float.MaxValue;
                bool get_elaps() {
                    var res = sw.Elapsed.Milliseconds < step * 200;
                    return res;
                }
                //TODO: correct TotalSeconds u need
                while (dist > 10 && get_elaps()) {
                    if (cond_err(out var ce1)) {
                        ui.AddToLog("SetCursor.." + info + ".. " + ce1, MessType.Warning);
                        break;
                    }
                    var cp = GetCursorPosition();
                    dist = tp.GetDistance(cp);
                    //var dir = (tp - cp);
                    var dir = tp.Subtract(cp);
                    dir = V2.Normalize(dir);
                    var nt = cp + (dir * dist / step);
                    SetCursorPos((int)nt.X, (int)nt.Y);
                    MouseMove();
                    //if(debug)
                    //    ui.AddToLog("SetCursor.." + info);
                    //TODO: correct next delay u need
                    Thread.Sleep(2);
                }
            }

            SetCursorPos((int)tp.X, (int)tp.Y); //last path part
            MouseMove();
            if (debug) {
                ui.AddToLog("SetCursor.." + info); //+" sp="+ sp.ToIntString()
                string dop = last_set_id.ToString();
                if (last_set_id > 9) {
                    if (last_set_id % 5 == 0)
                        dop = last_set_id.ToString();
                    else
                        dop = null;
                }
                spa.Add((tp, dop, left_down_count));
            }
            last_set_id += 1;
            last_setted = DateTime.Now;
            //using(var file = new StreamWriter(@"c:\Stas\mouse_log.txt", append: true)) {
            //    file.WriteLine((int)target.X + " " + (int)target.Y);
            //}
        }
        public static ConcurrentBag<(V2, string, int)> spa = new();
        static void MouseMove() {
            mouse_event((int)MouseEvents.Move, 0, 0, 0, 0);
        }
       
    }
}
