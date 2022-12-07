using V2 = System.Numerics.Vector2;

namespace Stas.GA{

    public class Hold : aTask {
        public override bool b_alt_stop => false;
        public Hold() {
            b_ignore_ui_busy = true; //prevent closing inventory
            ui.tasker.StopCastAll();
        }
        public override void Do() {
            if (ui.b_game_top && !ui.b_home && !ui.b_busy)
                ui.worker?.StopMoving(id_name);
            return; //dont move at all
        }
    }
}
