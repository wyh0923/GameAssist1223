using ImGuiNET;
using Color = System.Drawing.Color;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using System.Diagnostics;
using sh = Stas.GA.SpriteHelper;

namespace Stas.GA;
public partial class DrawMain {
    Color _purple = Color.FromArgb(100, Color.Purple);
    Color _orange = Color.FromArgb(100, Color.Orange);
    Color _red = Color.FromArgb(100, Color.Red);
    Color _green = Color.FromArgb(100, Color.Green);

    void DrawDebug() {
        //draw here not ingame debug info
        if (ui.curr_state != gState.InGameState)
            return;
        if (!ui.b_show_cell)
            DEbugTriggers();

        //DrawTestGP();

        if (ui.b_show_cell)
            DebugNavGridCellRouts(ui.MapPixelToGP);
        if (ui.nav.b_ready) {
            //DebugNavMesh();
            DebugNavPath();
            //DebugNavGP();
            //DebugNavTestPoints();
            DrawDebugNavCells();
            //DebugNeighbours();
            DrawTiles();
        }
       
        foreach (var v2 in ui.test.gpa) {
            DebugTgp(v2.Key, v2.Value, false);
        }
        if (ui.test.test_sp.Item1 != V2.Zero)
            DebugSP(ui.test.test_sp.Item1, true, ui.test.test_sp.Item2);
        foreach (var v2 in ui.test.spa) {
            DebugSP(v2.Key, true, v2.Value);
        }
        
       
        if (ui.sett.b_draw_mouse_moving)
            DrawMouseMoving();
        DrawSaveCell();
        //DrawBadCells();
        //DebugNotVisited();
        //DebugMapPixelToWorls();
        // DebugCursorToWorld3();
        if (ui.b_draw_save_screen)
            DrawSaveScreen();
        if (ui.b_draw_bad_centr)
            DrawBadCentr();
        DrawBotPath();
        DrawImportantTiles();
    }
    void DrawMouseMoving() {
        if (Mouse.spa.Count > 1) {
            var curr_list = Mouse.spa.ToArray();
            for (int i = 0; i < curr_list.Length; i++) {
                var sp = curr_list[i];
                map_ptr.AddCircleFilled(sp.Item1, 3.0f, ColorById(sp.Item3));
                if (!string.IsNullOrEmpty(sp.Item2))
                    map_ptr.AddText(sp.Item1.Increase(0, -16), Color.Gray.ToImgui(), sp.Item2);
                if (i > 0) {
                    var first = curr_list[i - 1];
                    map_ptr.AddLine(sp.Item1, first.Item1, ColorById(sp.Item3), 2);
                }
            }
        }
    }
    uint ColorById(int id) {
        var curr_id = id % 10;
        switch (curr_id) {
            case 0:
                return (Color.FromArgb(100, Color.Red).ToImgui());
            case 1:
                return (Color.FromArgb(100, Color.Green).ToImgui());
            case 2:
                return (Color.FromArgb(100, Color.Blue).ToImgui());
            case 3:
                return (Color.FromArgb(100, Color.Yellow).ToImgui());
            case 4:
                return (Color.FromArgb(100, Color.YellowGreen).ToImgui());
            case 5:
                return (Color.FromArgb(100, Color.Violet).ToImgui());
            case 6:
                return (Color.FromArgb(100, Color.BlueViolet).ToImgui());
            case 7:
                return (Color.FromArgb(100, Color.Magenta).ToImgui());
            case 8:
                return (Color.FromArgb(100, Color.DarkMagenta).ToImgui());
            case 9:
                return (Color.FromArgb(100, Color.Gold).ToImgui());
            default:
                return (Color.FromArgb(100, Color.Red).ToImgui());
        }
    }
    void DrawDebugNavCells() {
        foreach (var c in ui.nav.test_cells)
            DebugCell(c, _orange, true);
    }
    void DrawBadCells() {
        foreach (var c in ui.curr_map.danger_cells.Values)
            DebugCell(c, _orange, true);
    }
    void DebugNotVisited() {
        var need_check = ui.nav.grouts.Where(gc => !gc.b_visited).ToList();
        foreach (var n in need_check)
            DebugCell(n, _red, false);
    }
    void DrawSaveCell() {
        if (ui.test.test_cell != null) {
            lock (ui.test.test_cell) {
                DebugCell(ui.test.test_cell, _purple, true);
                DebugCell(ui.test.test_cell, _red, false);
                //var gc = ui.nav.Get_gc_by_gp(ui.test.test_cell.center);

            }
        }
    }
    void DrawImportantTiles() {
        var ta = ui.quest?.tiles;
        if (ta != null)
            foreach (var t in ta) {
                var ra = ui.nav.grouts.Where(r => r.fname == t).ToList();
                foreach (var r in ra)
                    DebugCell(r, Color.FromArgb(50, 200, 86, 15), true);
            }
    }
    void DrawTiles() {
        if (!ui.b_tile || ui.me == null || !ui.b_alt)
            return;
        var gc = ui.nav.Get_gc_by_gp(ui.me.gpos);
        Debug.Assert(gc != null);
        if (gc.fname == null)
            DebugCell(gc, Color.FromArgb(80, 200, 0, 15), false, "Error");
        else {
            DebugCell(gc, Color.FromArgb(80, 200, 86, 15), false, gc.fname);
            var ra = ui.nav.grouts.Where(r => r.fname == gc.fname);
            foreach (var r in ra) {
                DebugCell(r, Color.FromArgb(50, 200, 86, 15), true);

            }
        }
    }
   

    void DebugNavGridCellRouts(V2 gp) {
        //if (!ui.nav.b_ready) return;
        //var my_gc= ui.nav.Get_gc_by_gp(ui.me.gpos);
        //Draw(my_gc.routs, _green);

        var curs_gc = ui.nav.Get_gc_by_gp(gp);
        if (curs_gc == null) {
            ui.AddToLog("ui.nav.Get_gc_by_gp == null", MessType.Error);
            return;
        }
        var cell = curs_gc.Get_rout_by_gp(gp);
        var info = ui.nav.GetBitByGp(gp).ToString() + " v=" + cell?.b_visited;

        DebugTgp(gp, info);
        DebugCell(curs_gc, _orange);
        DrawCells(curs_gc.routs, _green);
        DrawCells(curs_gc.blocks, _purple);
    }
    void DrawCells(List<Cell> _cells, Color _color) {
        foreach (var r in _cells) {
            DebugCell(r, _color, false);
        }
    }
   
   
    void DebugNavPath() {
        if (ui.nav.debug_res == null)
            return;
        var rm = ui.MTransform();
        var list = ui.nav.debug_res?.path?.ToArray();
        if (list != null) {
            for (int i = 0; i < list.Length - 1; i++) {
                var a = V2.Transform(list[i].v2, rm);
                var b = V2.Transform(list[i + 1].v2, rm);
                map_ptr.AddLine(a, b, Color.DarkGreen.ToImgui(), 3);
                map_ptr.AddText(a.Increase(0, 10), Color.Gray.ToImgui(), i.ToString());
                var gc = ui.nav.Get_gc_by_gp(list[i].v2);
                DrawCells(gc.routs, Color.FromArgb(120, Color.Blue));
                //DebugCell(list[i].cell, Color.FromArgb(120, Color.GreenYellow), true);
            }
        }
    }
  
    void DEbugTriggers() {
        foreach (var t in ui.curr_map.triggers) {
            var red = Color.FromArgb(100, Color.DarkRed);
            if (!t.b_block)
                red = Color.FromArgb(100, Color.DarkGreen);
            DebugCell(t, red, true, "");
        }
    }
    void DebugSP(V2 sp, bool b_line = true, string info = null) {
        map_ptr.AddCircleFilled(sp, 3.0f, Color.Red.ToImgui());
        if (info != null)
            map_ptr.AddText(sp.Increase(0, -16), Color.Red.ToImgui(), info);
        var my_sp = V2.Transform(ui.me.gpos, ui.MTransform());
        if (b_line)
            map_ptr.AddLine(my_sp, sp, Color.Red.ToImgui(), 2f);
    }
    void DebugTgp(V2 tgp, string info = null, bool b_line = true) {
        var sp_tgp = V2.Transform(tgp, ui.MTransform());
        map_ptr.AddCircleFilled(sp_tgp, 3.0f, Color.Red.ToImgui());
        if (info != null)
            map_ptr.AddText(sp_tgp.Increase(0, -16), Color.Red.ToImgui(), info);
        if (b_line) {
            var my_sp = V2.Transform(ui.me.gpos, ui.MTransform());
            map_ptr.AddLine(my_sp, sp_tgp, Color.Red.ToImgui(), 2f);
        }
    }
    void DrawBotPath() {
        foreach (var b in ui.bots) {
            var gp_tr = V2.Transform(new V2(b.pos.X, b.pos.Y) * ui.worldToGridScale, ui.MTransform());
            var tgp_tr = V2.Transform(b.tgp, ui.MTransform());
            map_ptr.AddLine(gp_tr, tgp_tr, Color.Red.ToImgui(), 2f);
            map_ptr.AddCircleFilled(tgp_tr, 3.0f, Color.Red.ToImgui());
        }
    }
    void DrawWorldToSP(V3 v3) {
        var np = ui.WorldTPToSP(v3);
        map_ptr.AddCircleFilled(np, 8.0f, Color.Green.ToImgui());

    }
    void DebugMapPixelToWorls() {
        var v2 = ui.MapPixelToGP;
        DebugTgp(v2, "gp=" + v2.ToIntString());
    }
    void DebugCursorToWorld3() {
        var v3 = ui.MouseSpToWorld;
        DrawWorldToSP(v3);
        var dist = ui.me.pos.GetDistance(v3) * ui.worldToGridScale;
        ui.AddToLog("curs=[" + v3.ToIntString() + "] d=[" + dist.ToRoundStr(2) + "] " +
            "tgr=[" + ui.camera.target_ent?.RenderName + "]");
    }
    void DrawSaveScreen() {
        if (ui.gui.safe_screen == null)
            return;
        foreach (var b in ui.gui.safe_screen.Blocks.Values) {
            if (b != null) {
                map_ptr.AddQuadFilled(b.a, b.b, b.c, b.d,
                    Color.FromArgb(100, Color.OrangeRed).ToImgui());
                map_ptr.AddQuad(b.a, b.b, b.c, b.d,
                    Color.FromArgb(100, Color.Purple).ToImgui());
                if (!string.IsNullOrEmpty(b.info))
                    map_ptr.AddText(b.min.Increase(4, -10), Color.DarkGray.ToImgui(), b.info); //gc.id.ToString()
            }
        }
    }
    void DrawBadCentr() {
        var sz = ui.gui.safe_screen;
        if (sz == null)
            return;
        foreach (var s in sz.Centr) {
            DebugCell(s.Value, Color.OrangeRed, true, s.Key);
            DebugCell(s.Value, Color.Purple, false);
        }
        foreach (var s in sz.Rounts) {
            DebugCell(s.Value, Color.FromArgb(100, Color.Green), true, s.Key);
            DebugCell(s.Value, Color.FromArgb(100, Color.GreenYellow), false);
        }
    }
    void DebugCell(Cell gc, Color color, bool fill = false, string text = null) {
        var rm = ui.MTransform();
        var a = V2.Transform(gc.a, rm);
        var b = V2.Transform(gc.b, rm);
        var c = V2.Transform(gc.c, rm);
        var d = V2.Transform(gc.d, rm);
        if (fill)
            map_ptr.AddQuadFilled(a, b, c, d, color.ToImgui());
        map_ptr.AddQuad(a, b, c, d, color.ToImgui(), 2);
        var id_pos = V2.Transform(V2.Lerp(gc.max, gc.min, 0.5f), ui.MTransform());
        if (text != null) {
            ImGui.SetWindowFontScale(2f);
            map_ptr.AddText(id_pos, Color.DarkGray.ToImgui(), text); //gc.id.ToString()
            ImGui.SetWindowFontScale(1.0f);
        }
    }
}

