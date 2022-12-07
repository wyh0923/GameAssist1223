using System.IO;
using System.Reflection;
namespace Stas.GA; 

public partial class NavMesh {
    string fname { get {
            var exe_dir = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(exe_dir);
            return  Path.Combine(dir, @"Visited\" + ui.curr_map_hash + ".visited");
        } }
    (uint, DateTime) last_save;
    public void SaveVisited(bool _b_rewrite = false) {
        if (last_save.Item1 == ui.curr_map_hash 
            && last_save.Item2.AddSeconds(60) < DateTime.Now && !_b_rewrite) {
            return;
        }
        var visited = new List<VisitedGrid>();
        foreach (var gc in grid_cells.Where(gc => gc.visited_persent > 0)) {
            var vgc = new VisitedGrid(gc);
            foreach (var c in gc.routs) {
                if (c.b_visited)
                    vgc.visited.Add(new VisitedCell(c));
            }
            visited.Add(vgc);
        }
        last_save = (ui.curr_map_hash, DateTime.Now);
        FILE.SaveAsJson(visited, fname);
    }
    public void LoadVisited() {
        if (ui.curr_map_name.StartsWith("Azurite Mine")) {
            return;
        }
      
        if (File.Exists(fname)) {
            List<VisitedGrid> saved = null;
            sw.Restart();
            try {
                saved = FILE.LoadJson<List<VisitedGrid>>(fname);
            } catch (Exception ex) {
                ui.AddToLog("LoadVisited err: "+ex.Message, MessType.Error);
                return;
            }
            if (saved == null) {
                ui.AddToLog("LoadVisited: vant load from file=[" + fname + "]", MessType.Error);
                return;
            }
            foreach (var old_gc in saved) {
                var routs_area = 0f; //total(summ) area of all routs
                var visited_area = 0f; //total(summ) area of visited routs
                var curr_gc = grid_cells.FirstOrDefault(gc => old_gc.min_x == gc.min.X && old_gc.min_y == gc.min.Y
                   && old_gc.max_x == gc.max.X && old_gc.max_y == gc.max.Y);
                if (curr_gc == null)
                    continue;
                foreach (var r in curr_gc.routs) {
                    routs_area += r.area;
                }
                if (old_gc.visited == null)
                    continue;
                foreach (var gc in old_gc.visited) {
                    var rout = curr_gc.routs.FirstOrDefault(c => c.min.X == gc.min_x && c.min.Y == gc.min_y
                    && gc.max_x == c.max.X && gc.max_y == c.max.Y);
                    if (rout != null) {
                        rout.b_visited = true;
                        visited_area += rout.area;
                    }
                }
                if (routs_area > 0) {
                    curr_gc.visited_persent = visited_area / routs_area;
                    if (curr_gc.visited_persent == 1)
                        curr_gc.b_visited = true;
                }
            }
            ui.AddToLog("NavMesh visited loaded at: [" + sw.ElapsedTostring() + "]", MessType.Warning);
        }
    }
}
