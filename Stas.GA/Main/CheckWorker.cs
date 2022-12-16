using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Stas.GA;
using System.Text.Json.Serialization;


public partial class Settings : iSett {
    /// <summary>
    /// Manually match the hero to the preset ones
    /// </summary>
    public Dictionary<string, string> my_worker_names { get; set; } = new();
    public Settings() {
    }
}
public partial class ui {
    static Player curr_player;
    static void CheckWorker() {
        if (me.Address == IntPtr.Zero ) {
            //its possible if relogin fast - debug here here
            return;
        }
        if (b_worker_err) {
            return;
        }
        else {
            me.GetComp<Player>(out var _cp);//current me.player 

            if (worker == null || curr_player == null ||
                (_cp != null && _cp.Name != curr_player.Name)) {
                if (_cp != null && !string.IsNullOrEmpty(_cp.Name)) {
                    if (!b_worker_from_settings_err) {
                        if (ui.sett.my_worker_names.ContainsKey(_cp.Name)) {
                            ui.AddToLog("CheckWorker A suitable build has been found for:[" + _cp.Name + "] ");
                            var class_name = ui.sett.my_worker_names[_cp.Name];
                            worker = GetWorkerByName(class_name);
                            if (b_worker_err) {
                                ui.AddToLog("CheckWorker for:[" + _cp.Name + "] a class that does not exist\n in the code is selected ["+ class_name + "]");
                                b_worker_from_settings_err = true;
                                b_worker_err = false; //for check auto next frame
                            }
                        }
                    }
                    else {
                        worker = GetWorkerByName(_cp.Name);
                    }
                    if(!b_worker_err)
                        curr_player = _cp;
                }
            }
        }
    }
    static bool b_worker_from_settings_err = false;
    static bool b_worker_err = false;
    static aWorker GetWorkerByName(string _name) {
        if (!b_worker_err) {
            try {
                var asm = Assembly.GetExecutingAssembly();//.GetTypes();
                var anme = "Stas.GA." + _name;
                var handle = Activator.CreateInstance(asm.FullName, anme);
                var w = handle.Unwrap();
                Type t = w.GetType();
                MethodInfo method = w.GetType().GetMethod("Load");
                method = method.MakeGenericMethod(t);
                Type[] arg = { null }; //число аргументов должно соотвествовать числу аргуметов в методе
                aWorker res = (aWorker)method.Invoke(w, arg);
                return res;
            }
            catch (Exception ex) {
                b_worker_err = true;
                var pattern = "Could not load type '(.*?)' from assembly";
                var re = new Regex(pattern);
                var err = "GetWorkerByName err...";
                Debug.Assert(re.IsMatch(ex.Message));
                err += (string)re.Matches(ex.Message)[0].Groups[1].Value;
                ui.AddToLog(err, MessType.Critical);
                return null;
            }
        }
        return null;
    }
}
