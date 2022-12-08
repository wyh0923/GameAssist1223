using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Stas.GA; 

public partial class ui {
    static Player curr_player;
    static void CheckWorker() {
        if (me.Address == IntPtr.Zero ) {
            //its possible if relogin fast - debug here here
            return;
        }
       // me.GetComp<Actor>(out var actor);
       
        me.GetComp<Player>(out var _cp);//current me.player 
        if (b_worker_err) {
        }
        else {
            if (worker == null || curr_player == null ||
                (_cp != null && _cp.Name != curr_player.Name)) {
                if (_cp != null && !string.IsNullOrEmpty(_cp.Name)) {
                  
                    worker = GetWorkerByName(_cp.Name);
                    curr_player = _cp;
                }
            }
        }
    }
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
