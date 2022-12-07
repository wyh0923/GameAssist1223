namespace Stas.GA;
public partial class InputChecker {
    //temporary manuale using damagetotem
    void D3() {
        if (Keyboard.b_Try_press_key(Keys.D3, "InputChecker D3", 900)) {
            if (ui.curr_role == Role.Master) {
                ui.SendToBots(Opcode.UseTotem, ui.MouseSpToWorld.ToByte());
            }
        }
    }
}

