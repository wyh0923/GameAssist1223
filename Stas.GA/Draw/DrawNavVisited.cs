#region using
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using Color = System.Drawing.Color;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
#endregion

namespace Stas.GA {
    partial class DrawMain {
        public void DrawNavVisited() {
            if (!ui.nav.b_ready || ui.b_contrl)  //&& ui.b_draw_debug) || ui.b_alt
                return;
            var rm = ui.MTransform();
            var v_volor = Color.FromArgb(25, Color.GreenYellow).ToImgui();
            var nv_color = Color.FromArgb(40, Color.Gold).ToImgui(); //test color
            var cc = v_volor;
            var vrc = 0;// visited  routs count
            var rc = 0;
            foreach (var gc in ui.nav.grid_cells) {
                //if(gc.routs_weight_with_neibor > 4.4f) {
                //    var a = V2.Transform(gc.a, rm);
                //    var b = V2.Transform(gc.b, rm);
                //    var c = V2.Transform(gc.c, rm);
                //    var d = V2.Transform(gc.d, rm);
                //    bptr.AddQuadFilled(a, b, c, d, tcolor);
                //    // bptr.AddQuad(a, b, c, d, tcolor, 3);
                //    var tpos = V2.Transform(gc.center, rm); 
                //    bptr.AddText(tpos, Color.Gray.ToImgui(), gc.routs_weight_with_neibor.ToRoundStr(1));
                //}
                foreach (var cl in gc.routs) {
                    rc += 1;
                    if (!cl.b_visited)
                        continue;
                    var a = V2.Transform(cl.a, rm);
                    var b = V2.Transform(cl.b, rm);
                    var c = V2.Transform(cl.c, rm);
                    var d = V2.Transform(cl.d, rm);
                    //bptr.AddQuad(a, b, c, d, bcolor, 3);
                    if (!cl.b_visited)
                        cc = nv_color;
                    map_ptr.AddQuadFilled(a, b, c, d, cc);
                    vrc += 1;
                }
            }
            //ui.AddToLog("gs=["+ ui.nav.grid_cells.Count+] rout=["+ rc + "] visited=["+ vrc + "]");//3803 613| 1521 836 110
        }
    }
}
