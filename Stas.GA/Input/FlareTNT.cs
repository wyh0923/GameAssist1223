using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;

public partial class InputChecker {
    void UseFlareTNT() {
        //TODO temporary disablked
        if (Keyboard.b_Try_press_key(Keys.R, "ICh R", 1200)) {
           // ui.tasker.TaskPop(new UseFlare(true));
        }

        if (Keyboard.b_Try_press_key(Keys.Z, "ICh, Z", 1200)) {
           // ui.tasker.TaskPop(new UseTNT());
        }
    }
}
