using Microsoft.Extensions.Primitives;
using System.Text;

namespace Stas.GA;

public partial class GameUiElements : Element {
    public Element KiracMission = new Element("KiracMission") ;
    public Element open_right_panel = new Element("OpenRightPanel");
    public Element open_left_panel = new Element("OpenLeftPanel") ;
    public Element passives_tree = new Element("passives_tree") ;
    public NpcDialog NpcDialog = new NpcDialog();
    public Element LeagueNpcDialog = new Element("LeagueNpcDialog") ;
    public Element BetrayalWindow = new Element("BetrayalWindow") ;
    public Element AtlasSkillPanel = new Element("AtlasSkillPanel") ;
    public Element DelveWindow = new Element("DelveWindow");
    public Element TempleOfAtzoatl = new Element("TempleOfAtzoatl");
    public Element AtlasPanel = new Element("AtlasPanel") ;

    bool _bbi;
    public string b_busy_info { get; private set; }
    List<Element> need_check_vis;
    StringBuilder sb = new StringBuilder();
    object locker = new object();
    /// <summary>
    /// same time make debug string b_busy_info
    /// </summary>
    public bool b_busy {
        get {
            lock (locker) {
                sb.Clear();
                _bbi = false;
                foreach (var e in need_check_vis) {
                    if (e.IsValid) {
                        if ((e.tName == "open_left_panel" || e.tName == "open_right_pane")  && e.IsVisibleLocal)
                            _bbi = true;
                        else {
                            if (e.IsVisible)
                                _bbi = true;
                        }
                      
                        sb.AppendLine(e.tName + "=[" + e.IsVisible + "]");
                    }
                    else
                        sb.AppendLine(e.tName + " NOT valid");
                }
                b_busy_info = sb.ToString() ;
            }
            return _bbi;
        }
    }
}
