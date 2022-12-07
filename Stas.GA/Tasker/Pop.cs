namespace Stas.GA;

public abstract partial class aTasker {
    public void TaskPop(aTask _nt, bool cleare_debug = true) {
        if (_nt == null) {
            //Debug.Assert(new_task != null);
            //ui.AddToLog("TaskPop.. new task==null", MessType.Error);
            return;
        }
        if (_nt.error != TaskErrors.NoError) {
            if (cleare_debug)
                CleareDebug(_nt);
            var _mess = _nt.error.ToString();
            if (!string.IsNullOrEmpty(_nt.last_error))
                _mess = _nt.last_error;
            ui.AddToLog("TaskPop=>" + _nt.id_name + "..." + _mess, MessType.Error);
            return;
        }
        if (ui.b_grace && !_nt.b_dont_w8_grace) {
            ui.AddToLog("Cant TaskPop: we have Grace", MessType.Error);
            return;
        }
        var ntt = _nt.GetType();
        if (task != null) {
            var inlist = tasks.Any(t => t.GetType() == ntt);
            if (task?.GetType() == ntt || inlist) {
                if (cleare_debug)
                    CleareDebug(_nt);
                ui.AddToLog("TaskPop.. try add same(in list mb) task: " + ntt, MessType.Warning);
                return;
            }
            tasks.Push(task); //set the current task as secondary
        }
        task = _nt;
    }
}