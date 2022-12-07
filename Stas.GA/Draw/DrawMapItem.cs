#region using

using Color = System.Drawing.Color;
using System.Drawing;
using System.Linq;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using System.Diagnostics;
using ImGuiNET;

#endregion

namespace Stas.GA {
    partial class DrawMain {
        void DrawMapItem(aMapItem ami) {
            if (ami.uv == RectangleF.Empty) {
                ui.AddToLog("DrawMapItem err: uv==empty for: " + ami.ToString(), MessType.Error);
                return;
            }
            if (ui.b_only_unknow && ami.mii != MapIconsIndex.unknow)
                return;
            var rm = ui.MTransform();
            var his = ami.size / 2;
            his *= ui.sett.map_scale;
            var mi_gpos = ami.pos * ui.worldToGridScale;
            var pos = V2.Transform(new V2(mi_gpos.X, mi_gpos.Y), rm);
            var p_min = new V2(ami.uv.Left, ami.uv.Top);
            var p_max = new V2(ami.uv.Left + ami.uv.Width, ami.uv.Top + ami.uv.Height);
            map_ptr.AddImage(icons, pos.Increase(-his, -his), pos.Increase(his, his), p_min, p_max);

            if (ami is StaticMapItem) {
                var smi = (StaticMapItem)ami;
                if (smi.b_done) {
                    var uv = SpriteHelper.GetUV(MapIconsIndex.done);
                    map_ptr.AddImage(icons, pos.Increase(-his, -his), pos.Increase(his, his),
                        new V2(uv.Left, uv.Top),
                        new V2(uv.Left + uv.Width, uv.Top + uv.Height));
                }
                if (smi.remn != null) {
                    var pval = smi.remn.positive.Sum(p => p.Value);
                    map_ptr.AddText(pos.Increase(-5, -15), Color.White.ToImgui(), pval.ToString());

                    var nval = smi.remn.negative.Sum(p => p.Value);
                    map_ptr.AddText(pos.Increase(-5, 0), Color.White.ToImgui(), nval.ToString());
                }
            }
          
            if (ui.b_contrl) {
                if (ami is MapItem && ami != null && ami.ent?.danger_rt > 0) {
                    var lt = pos.Increase(-his, -0.7f * his);
                    var rt = pos.Increase(20, -0.7f * his);
                    var lb = pos.Increase(-his, 0.7f * his + 3);
                    var rb = pos.Increase(20, 0.7f * his + 3);
                    map_ptr.AddQuadFilled(lt, rt, rb, lb, Color.Red.ToImgui());
                    map_ptr.AddText(pos.Increase(0, -his), Color.White.ToImgui(),
                        ami.ent.danger_rt.ToRoundStr(3));

                    if (ami.ent.GetComp<Positioned>(out var test)) {
                        var past = V2.Transform(test.past_pos * ui.worldToGridScale, ui.MTransform());
                        var curr = V2.Transform(test.curr_pos * ui.worldToGridScale, ui.MTransform());
                        var next = V2.Transform(test.next_pos * ui.worldToGridScale, ui.MTransform());

                        map_ptr.AddLine(past, curr, Color.Red.ToImgui(), 2);
                        map_ptr.AddLine(curr, next, Color.Blue.ToImgui(), 2);
                    }

                }
                var info = ami.ent?.eType+ " id=" + ami.ent?.id;
                map_ptr.AddText(pos.Increase(his / 2, -8), Color.DarkGray.ToImgui(), info) ;
            }

            if (ui.b_alt) {// 
                var info = ami.info; 
                if(string.IsNullOrEmpty(info))
                    info= "id=" + ami.ent?.id;
                if (ami is StaticMapItem) {
                    info += " [St]";
                }
                map_ptr.AddText(pos.Increase(his / 2, -8), Color.LightGreen.ToImgui(), info);
            }
        }
    }
}
