using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
namespace Stas.GA;

public partial class InputChecker : aMouseChecker , IDisposable{
    Stopwatch sw = new Stopwatch();
    public int w8 { get; } = 16;////1000 / 60 = 16
    StringBuilder sb = new StringBuilder();
    Random R = new Random();
    Thread inp_thread ;
    public InputChecker() {
        inp_thread = new Thread(() => {
            while (ui.b_running) {
              
                F1(); //call sameone for debug
                if (Keyboard.b_Try_press_key(Keys.F11, "ICh F11")) {
                    ui.b_minimize = !ui.b_minimize;
                }
                if (ui.b_contrl && Keyboard.b_Try_press_key(Keys.F12, "ICh F12")) {
                    ui.b_show_info_over = !ui.b_show_info_over;
                }
                var b_game_top = ui.b_game_top || ui.b_imgui_top;

                if (!b_game_top || !ui.b_ingame || ui.b_busy) { //|| !ui.b_trade_top
                    Thread.Sleep(100);
                    continue;
                }
                if (ui.b_game_top || ui.b_imgui_top)
                    SetMouseDeltaSumm();
                else
                    ClearDeltas();

                SetMapOffset();
                ManualSkillUsing();
                //ControlManualUsing();
                F2(); //transit
                if (Keyboard.b_Try_press_key(Keys.D2, "jump")) {
                    //var trg = ui.MouseSpToWorld;
                    //Debug.Assert(trg.X > 0 && trg.Y > 0);
                    //ui.SendToBots(Opcode.Jump, trg.ToByte());
                }
                if (Keyboard.b_Try_press_key(Keys.Q, "Left flask", 900)) {
                    //Keyboard.KeyPress(Keys.F5);
                    //ui.SendToBots(Opcode.KeyPress, Keys.F5.ToByteArr());
                    //Thread.Sleep(60 + R.Next(0, 60));
                    //Keyboard.KeyPress(Keys.F6);
                    //ui.SendToBots(Opcode.KeyPress, Keys.F6.ToByteArr());
                }
                if (Keyboard.b_Try_press_key(Keys.W, "Right flask", 900)) {
                    //Keyboard.KeyPress(Keys.F7);
                    //ui.SendToBots(Opcode.KeyPress, Keys.F7.ToByteArr());
                    //Thread.Sleep(60 + R.Next(0, 60));
                    //Keyboard.KeyPress(Keys.F8);
                    //ui.SendToBots(Opcode.KeyPress, Keys.F8.ToByteArr());
                }
                if (Keyboard.b_Try_press_key(Keys.F3, "InputChecker")) {
                    ui.tasker?.Unhold();
                    if (ui.curr_role == Role.Master) {
                        ui.SendToBots(Opcode.UnHold);
                    }
                }
                if (Keyboard.b_Try_press_key(Keys.F4, "InputChecker")) {
                    ui.tasker?.Hold();
                    if (ui.curr_role == Role.Master) {
                        ui.SendToBots(Opcode.Hold);
                    }
                }
                if (Keyboard.b_Try_press_key(Keys.F, "InputChecker")) {
                    ui.tasker?.Unhold();//if this run directly on a bot PC, for example for testing purposes
                    if (ui.curr_role == Role.Master) {
                        if (ui.b_home) {
                            ui.SendToBots(Opcode.NavGo, ui.me.gpos.ToByte());
                        }
                        else {
                            ui.SendToBots(Opcode.FallowHard, true);
                        }
                    }
                    else
                        ui.tasker?.SetFallowHard(true);
                }
                if (Keyboard.b_Try_press_key(Keys.G, "InputChecker")) {
                    //ui.tasker.Unhold();
                    //var gpos = ui.MapPixelToGP;
                    //ui.tasker.TaskPop(new NavGo(gpos, null));
                    //if (ui.curr_role == Role.Master) {
                    //    ui.SendToBots(Opcode.NavGo, gpos.ToByte());
                    //}
                }
                D3(); //use damage totem
                UseFlareTNT();
                if (ui.curr_role == Role.Master) {
                    var kpa = new List<Keys>() { Keys.Space, Keys.E, Keys.V }; //Keys.I Keys.Y Keys.G
                    for (int i = 0; i < kpa.Count; i++) {
                        if (Keyboard.b_Try_press_key(kpa[i], "InputChecker", 600)) {
                            ui.SendToBots(Opcode.KeyPress, kpa[i].ToByteArr(), false);
                        }
                    }
                }

                if (Keyboard.b_Try_press_key(Keys.F9, "ICh F9")) {
                    //ui.worker.SavingAss();
                    //ui.SendToBots(Opcode.SavingAss);
                }
              
                if (Keyboard.b_Try_press_key(Keys.F10, "ICh F10") && ui.curr_role == Role.Master) {
                    ui.b_draw_bots = !ui.b_draw_bots;
                }
                if (Mouse.IsButtonDown(Keys.LButton)) {
                    //var uw = ui.gc_ui.Ultimatum;
                    //if (uw.IsVisible) {
                    //    var okb = uw.confirm;
                    //    if (okb != null && okb.Parent != null && okb.Parent.b_mouse_over) {
                    //        ui.SendToBots(Opcode.SetUltim, new byte[] { (byte)uw.selected_choice });
                    //    }
                    //}
                }

                if (Keyboard.b_Try_press_key(Keys.Oemplus, "ICh Oem+")) {
                    //ui.mapper.AddNPC();
                }

                if (ui.sett.b_use_ingame_map) {
                    //if (Keyboard.IsKeyDown(Keys.NumPad5, "ICh")) {
                    //    ui.map_offset = V2.Zero;
                    //}
                    if (Keyboard.IsKeyDown(Keys.NumPad8, "ICh")) {
                        ui.map_offset.Y += 0.1f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad2, "ICh")) {
                        ui.map_offset.Y -= 0.1f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad4, "ICh")) {
                        ui.map_offset.X += 0.1f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad6, "ICh")) {
                        ui.map_offset.X -= 0.1f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad7, "ICh")) {
                        ui.sett.map_angle = ui.sett.map_angle += 0.05f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad9, "ICh")) {
                        ui.sett.map_angle = ui.sett.map_angle -= 0.05f;
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad1, "ICh zoom_in")) {
                        ui.sett.map_scale = Math.Clamp(ui.sett.map_scale += 0.01f, 0.5f, 20);
                    }
                    if (Keyboard.IsKeyDown(Keys.NumPad3, "ICh zoom_out")) {
                        ui.sett.map_scale = Math.Clamp(ui.sett.map_scale -= 0.01f, 0.5f, 20);
                    }
                }
                else {
                    if (ui.curr_map.danger > 0) {
                        ui.sett.map_scale = ui.sett.map_scale_def;
                    }
                    else {
                        if (ui.sett.b_use_keybord_for_zoom) {
                            if (Keyboard.IsKeyDown(ui.sett.zoom_in, "ICh zoom_in")) {
                                ui.sett.map_scale = Math.Clamp(ui.sett.map_scale += 0.01f, 0.5f, 20);
                            }
                            if (Keyboard.IsKeyDown(ui.sett.zoom_out, "ICh zoom_out")) {
                                ui.sett.map_scale = Math.Clamp(ui.sett.map_scale -= 0.01f, 0.5f, 20);
                            }
                        }
                        else {
                            if (Mouse.IsButtonDown(Keys.XButton1)) {
                                ui.sett.map_scale = Math.Clamp(ui.sett.map_scale += 0.2f, 0.5f, 20);
                            }
                            if (Mouse.IsButtonDown(Keys.XButton2)) {
                                ui.sett.map_scale = Math.Clamp(ui.sett.map_scale -= 0.2f, 0.5f, 20);
                            }
                        }
                    }
                }
                #region w8ting
                var t_elaps = (int)sw.Elapsed.TotalMilliseconds; //totale elaps
                if (t_elaps < w8) {
                    Thread.Sleep(w8 - t_elaps);
                }
                if (t_elaps > w8)
                    ui.AddToLog("ui warning! Big tick time=[" + t_elaps + "]");
                #endregion
                Thread.Sleep(1000 / 120);
            }
        });
        inp_thread.IsBackground = true;
        inp_thread.Start();
    }
    void ManualSkillUsing() {
        if (!ui.b_town && ui.worker != null) {
            foreach (var s in ui.worker.my_skills) {
                if (Keyboard.IsKeyDown(s.key)) {
                    if (!s.b_ready) {
                        ui.AddToLog("try reg manual using of a skill err [" + s.tName + "] not ready");
                    }
                    s.WasUsedManual();
                }
            }
        }
    }
    bool b_was_clicked;
    public void SetMapOffset() {
        if (Mouse.IsButtonDown(Keys.RButton)) //ui.b_contr_alt &&
            ui.map_offset = V2.Zero;
        if (!Mouse.IsButtonDown(Keys.LButton))
            b_was_clicked = false;
        if (ui.b_imgui_top && ui.b_contr_alt && ui.b_shift && Mouse.IsButtonDown(Keys.LButton)) {
            if (!b_was_clicked) {
                b_was_clicked = true;
                var raw_gp = ui.MapPixelToGP;
                ui.map_offset = new V2(raw_gp.X, -raw_gp.Y);
            }
        }
    }
    void IDisposable.Dispose() {
        inp_thread.Abort();
    }
}
