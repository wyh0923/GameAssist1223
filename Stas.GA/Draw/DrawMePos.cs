#region using
using System;
using System.Drawing;
using System.Linq;
using ImGuiNET;
using V2 = System.Numerics.Vector2;
#endregion
namespace Stas.GA {
    partial class DrawMain {
        void DrawMePos() {
            var cpa = ui.curr_map.me_pos.ToArray();//thread safe copy of
            if(cpa.Length <4)
                return;
            var rm = ui.MTransform();
            for (int i=0; i< cpa.Length - 1; i++) {
                var p1 = V2.Transform(cpa[i], rm); 
                var p2 = V2.Transform(cpa[i + 1], rm);
                map_ptr.AddLine(p1, p2, Color.Red.ToImgui(), 2f);
            }
        }
    }
}
