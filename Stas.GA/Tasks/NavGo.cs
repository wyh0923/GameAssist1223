namespace Stas.GA;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
internal class NavGo : aTask {
    NavRes nav_res;
    V2 tgp;//the current point obtained from the navigation path. if I'm near her, I'll add her to my visited
    V2 nav_gp;//last point i need
    public NavGo(V2 gp, aTask _parent, Action _do_after = null) {
        if (ui.nav.b_can_hit(gp)) {
            var curr_gdist = ui.me.gpos.GetDistance(gp);
            if (curr_gdist < 100) {
                error = TaskErrors.init_error;
                last_error = "Don't need use nav here";
                return;
            }
        }
        nav_gp = gp;
        b_need_auto_stop = true;
        b_debug = true; //for debug tgp
        if (gp == V2.Zero) {
            error = TaskErrors.init_error;
            last_error = "gp == V2.Zero";
            return;
        }
        do_after = _do_after;
        parent = _parent;
        if (parent != null)
            b_danger_stop = parent.b_danger_stop;
        else
            b_danger_stop = !ui.sett.b_can_pull_alone;
        ui.tasker.Add_iTask(new MyTask(id, gp, id_name));
        nav_res = ui.nav.GetRes(me.gpos, gp);
    }

    public override void Do() {
    }
}
