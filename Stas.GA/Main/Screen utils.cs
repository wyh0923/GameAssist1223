#region using
using System;
using System.Diagnostics;
using System.Numerics;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using System.Drawing;
#endregion

namespace Stas.GA {
    public partial class ui {
        static object locker = new object();
        public static V2 map_offset;
        public static Rectangle game_window_rect => EXT.GetWindowRectangle(game_ptr);
        static V2[] pa;
        static V2 GetInterpPos() {
            lock (locker) {
                pa = curr_map.me_pos.ToArray();//this thread safe copy of
            }
            if (pa.Length < 4)
                return new V2(me.gpos.X, -me.gpos.Y);
            float t = 0.4f;
            var l = pa.Length;
            var p = pa[l-4];//oldest
            var p1 = pa[l-3];//oldest
            var p2 = pa[l-2];//oldest
            var p3 = pa[l-1];

            var m1 = V2.Lerp(p, p1, t);
            var m2 = V2.Lerp(p1, p2, t);
            var m3 = V2.Lerp(p2, p3, t);

            var l1 = V2.Lerp(m1, m2, t);
            var l2 = V2.Lerp(m2, m3, t);

            var res= V2.Lerp(l1, l2, t);
            var same = true;
            foreach (var _cp in pa) {
                if (_cp != res) { same = false; break; }
            }
            //AddToLog("InterpPos=[" + same + "]");
            return new V2(res.X, -res.Y);
        }
        public static  Matrix3x2 MTransform() {
            var rect = game_window_rect;
            var scp = new V2((float)rect.Width  / 2+ rect.X, 
                (float)rect.Height  / 2 + rect.Y); //screen center
            if (me == null)
                return Matrix3x2.Identity;// * Matrix3x2.CreateScale(ui.sett.map_scale, scp);
            var cp = new V2(me.gpos.X, -me.gpos.Y);
            if (ui.sett.b_map_interpolate)
                cp = GetInterpPos(); 
            if (map_offset != V2.Zero)
                cp = map_offset;
            var dir = scp - cp;
            var ra = -sett.map_angle * Math.PI / 180; // Convert to radians
            var mirrm = new Matrix3x2() { M11 = 1f, M22 = -1f };
            var res = mirrm * Matrix3x2.CreateTranslation(dir)
                * Matrix3x2.CreateScale(sett.map_scale, scp) //
                * Matrix3x2.CreateRotation((float)ra, scp);
            //return Matrix3x2.Identity;
            return res;
        }

        public static V2 MapPixelToGP {
            get {
                Matrix3x2.Invert(MTransform(), out var im);
                return V2.Transform(Mouse.GetCursorPosition(), im);
            }
        }
        public static V3 MapPixelToWorld {
            get {
                var v2 = MapPixelToGP;
                var h = Get_H_from_gp(v2);
                var conv = v2 * gridToWorldScale;
                return new V3(conv.X, conv.Y, h);
            }
        }
        /// <summary>
        /// Z coordinate wold be wrong - its temporary relise
        /// </summary>
        public static V3 MouseSpToWorld { get {
                Debug.Assert(camera != null && camera.IsVaild);
                return camera.CursorToWorld;
            } } 
        public static V2 MouseSpToGP {
            get {
                var ctw = MouseSpToWorld;
                return new V2(ctw.X, ctw.Y) * worldToGridScale;
            }
        }
        public static float Get_H_from_gp(V2 gp) {
            var h = (me != null) ? me.pos.Z : 0;//TODO: this old not correct method
            var ghd = curr_map.GridHeightData;
            if (ghd == null)
                return h;
            if (gp.X > curr_map.cols || gp.Y > curr_map.rows || gp.X < 0 || gp.Y < 0) {
                AddToLog("Get_H_from_gp => gp out of Map", MessType.Error);
                return h;
            }
            var x = (int)gp.X;
            var y = (int)gp.Y;
            h = ghd[y][x];
            return h;
        }
        //TODO: try use WorldTPToSP instead of this
        //blood aqueduct Y = 4*X
        public static V2 TgpToSP(V2 tgp) {
            var conv = tgp * gridToWorldScale;
            var h = Get_H_from_gp(tgp);
            return WorldTPToSP(new V3(conv.X, conv.Y, h));
        }
        public static float screen_k => EXT.GetScreenScalingFactor();
        static bool b_full_screen {
            get {
                if (game_ptr == IntPtr.Zero) return true;
                var desktop_rect = EXT.GetWindowRectangle(EXT.GetDesktopWindow());
                var pw_rect = EXT.GetWindowRectangle(game_ptr);
                return desktop_rect.Width == pw_rect.Width && desktop_rect.Height == pw_rect.Height;
            }
        }
        public static V2 WorldTPToSP(V3 v3) {
            var sp = camera.WorldToScreen(v3) / screen_k + w_offs;
            return sp;
        }
        public static V2 WorldTPToSP_safe(V3 v3) {
            var sp = camera.WorldToScreen(v3) / screen_k + w_offs;
            return SpToSp_safe(sp);
        }
        public static V2 SpToSp_safe(V2 sp) {
            if (gui.safe_screen == null)
                return sp;
            var cp = game_window_rect.Center();
            foreach (var b in gui.safe_screen.Blocks.Values) {
                if(b ==null)
                    continue;
                var hit = b.LS_intersection(new V2(cp.X, cp.Y), sp);
                if (hit.Count >0) {
                    var corrected = V2.Lerp(cp, hit[0], 0.95f);
                    //Debug.Assert(b_sp_is_safe(corrected));
                    return corrected;
                }
            }
            return sp;
        }
        public static V2 w_offs {
            get {
                var add = new V2(7, 32); //title of game window
                if (b_full_screen)
                    add = V2.Zero;
                return new V2(game_window_rect.Left + add.X, game_window_rect.Top + add.Y);
            }
        }
    }
}
