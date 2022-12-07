using Vector2 = System.Numerics.Vector2;
using Color = System.Drawing.Color;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Stas.GA;
public abstract class ConfigLineBase {

    public string Text { get; set; }
    public Color? Color { get; set; }

    public override bool Equals(object obj) {
        return Text == ((ConfigLineBase)obj).Text;
    }

    public override int GetHashCode() {
        return Text.GetHashCode();
    }
    public override string ToString() {
        return Text + "c=" + Color.ToString();
    }
}
public class PreloadInfo {
    public string DisplayName { get; set; }
    public Color4 Color { get; set; }

    public override string ToString() {
        return DisplayName + "[" + Color.ToString() + "]";
    }
}
public class PreloadAlert {
    public string ttype => GetType().Name.ToString();
    readonly object _locker = new object();

    public List<PreloadInfo> DrawAlerts = new List<PreloadInfo>();
    Dictionary<string, PreloadInfo> all_alerts = new Dictionary<string, PreloadInfo>();
    List<PreloadInfo> curr_alerts = new List<PreloadInfo>();
    public bool b_ready;
    public PreloadAlert() {
        ReloadConfig();
    }

    public void AreaChange() {
        lock (_locker) {
            DrawAlerts.Clear();
        }
        curr_alerts.Clear();
        if (ui.b_home || ui.b_town)
            return;
        Parse();
    }
     string fname {
        get {
            var strExeFilePath = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(strExeFilePath);
            var n = Path.Combine(dir, ui.sett.preload_fname);
            return n;
        }
    }
    public void ReloadConfig() {
        if (File.Exists(fname)) {
            all_alerts = FILE.LoadJson<Dictionary<string, PreloadInfo>>(fname);
        }
        else {
            ui.AddToLog("preload err: not found file:" + fname, MessType.Critical);
        }
        Parse();
    }
    void Parse() {//~2-3ms
        b_ready = false;
        var sw = new SW("PA Parse");
        sw.Restart();
        var allFiles = ui.curr_loaded_files.PathNames;
        foreach (var f in allFiles) {
            if (all_alerts.ContainsKey(f.Key)) {
                lock (_locker) {
                    curr_alerts.Add(all_alerts[f.Key]);
                }
            }
        }
        lock (_locker) {
            DrawAlerts = curr_alerts;
        };
        b_ready = true;
        sw.Print();
    }
    void DebugSame() {
        var fname = @"C:\log\Cameria.txt";
        File.WriteAllText(fname, "");
        var sw = File.AppendText(fname);
        var allFiles = ui.curr_loaded_files.PathNames;
        foreach (var kv in allFiles) {
            Debug.Assert(kv.Value == ui.area_change_counter.Value);
            var key = kv.Key;
            if (key.Contains('@')) key = key.Split('@')[0];
            if (key.Contains("Cameria")) {
                sw.WriteLine(key);
            }
            //if (text.Contains("Dannig") || text.Contains("Rog")
            //|| text.Contains("Tujen") || text.Contains("Gwennen")) {
            //    sw.WriteLine(text);
            //}
        }
        sw.Dispose();
    }
}
   