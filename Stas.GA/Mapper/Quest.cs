using System.Diagnostics;
using System.IO;
using V2 = System.Numerics.Vector2;
namespace Stas.GA; 
public class Quest : iSave {
    public override string fname {
        get {
            Debug.Assert(Directory.Exists("Quests"));
            var n = "Quests\\" + ui.curr_world.world_area.Id + ".quest";
            return n;
        }
    }
    public List<string> Loot_names { get; set; }
    public List<aTask> Tasks { get; set; }
    public List<Tuple<int, int, string>> npcs { get; set; }
    public List<string> tiles { get; set; }
    public List<Tuple<int, int, DateTime>> Way_points { get; set; }
    public List<Tuple<int, int, DateTime>> Trials { get; set; }
    public Quest() {
        if (!Directory.Exists("Quests"))
            Directory.CreateDirectory("Quests");
        Tasks = new List<aTask>();
        tiles = new List<string>();
        npcs = new List<Tuple<int, int, string>>();
        Loot_names = new List<string>();
        Way_points = new List<Tuple<int, int, DateTime>>();
    }
}
