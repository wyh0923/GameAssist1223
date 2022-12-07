using System.IO;
using System.Reflection;

namespace Stas.GA;

public class Quest : iSave{
    public override string fname { get {
            var strExeFilePath = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(strExeFilePath);
            var n = Path.Combine(dir, "Quests\\" + ui.curr_world.world_data.Id + ".quest");
            return n;
        } }
    public List<string> Loot_names { get; set; }
    public List<aTask> Tasks { get; set; }
    public List<Tuple<int, int, string>> npcs { get; set; }
    public List<string> tiles { get; set; }
    public List<Tuple<int, int, DateTime>> Way_points { get; set; }
    public List<Tuple<int, int, DateTime>> Trials { get; set; }
    public Quest()
    {
        Tasks = new List<aTask>();
        tiles = new List<string>();
        npcs = new List<Tuple<int, int, string>>();
        Loot_names = new List<string>();
        Way_points = new List<Tuple<int, int, DateTime>>();

    }
}
