using System.Diagnostics;
using System.Runtime.InteropServices;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

/// <summary>
///     This is actually UiRoot main child which contains
///     all the UiElements (100+). Normally it's at index 1 of UiRoot.
///     This class is created because traversing childrens of
///     UiRoot is a slow process that requires lots of memory reads.
///     Drawback:
/// </summary>
public partial class GameUiElements : Element {
    Thread worker;
    public SafeScreen safe_screen;
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPassiveTreePtr")]
    static extern IntPtr GetPassiveTreePtr();
    internal GameUiElements(IntPtr ptr) 
        : base(ptr, "gui") {
        MakeNeedCheckVisList();
        worker = new Thread(() => {
            while (ui.b_running) {
                if (ui.curr_state != GameStateTypes.InGameState) {
                    Thread.Sleep(ui.w8*10);
                    continue;
                }
                Init(tName+"worker");
                base.Tick(Address, tName + "worker");
                Thread.Sleep(100);
            }
        });
        worker.IsBackground= true;
        worker.Start(); 
    }
    internal void MakeNeedCheckVisList() {
        need_check_vis = new List<Element>() { KiracMission, open_left_panel, open_right_panel,
            passives_tree, NpcDialog, LeagueNpcDialog, BetrayalWindow, 
            AtlasPanel, AtlasSkillPanel,DelveWindow,TempleOfAtzoatl };
        if (!ui.sett.b_use_ingame_map)
            need_check_vis.Add(large_map);

    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetGuiOffsets")]
    public static extern int GetGuiOffsets(IntPtr gui_ptr, ref guiOffset offs);
    override protected void Init(string from) {
        base.Init(from);
        Debug.Assert(Address != default);
        var data = new guiOffset();
        GetGuiOffsets(Address, ref data);
        ui_flask_root.Tick(data.ui_flask_root, tName);
        KiracMission.Tick(data.KiracMission, tName);
        open_right_panel.Tick(data.open_right_panel, tName);
        open_left_panel.Tick(data.open_left_panel, tName);
        var pt_ptr = GetPassiveTreePtr();
        if (pt_ptr != default)
            passives_tree.Tick(pt_ptr, tName + ".GetPassiveTree");
        else {
            ui.AddToLog(tName + ".GetPassiveTree err=bad ptr", MessType.Error);
        }
        //NpcDialog.Tick(data.NpcDialog, tName);
        LeagueNpcDialog.Tick(data.LeagueNpcDialog, tName);
        BetrayalWindow.Tick(data.BetrayalWindow, tName);
        maps_root.Tick(data.maps_root_ptr, tName);
        large_map.Tick(maps_root.children_pointers[0], tName);
        AtlasPanel.Tick(data.AtlasPanel, tName);
        AtlasSkillPanel.Tick(data.AtlasSkillPanel, tName);
        DelveWindow.Tick(data.DelveWindow, tName);
        TempleOfAtzoatl.Tick(data.TempleOfAtzoatl, tName);
        if_I_dead.Tick(data.if_i_dead, tName);
        world_map.Tick(data.WorldMap, tName);
        stash_element.Tick(data.StashElement, tName);
        QuestRewardWindow.Tick(data.QuestRewardWindow, tName);
        //var data2 = reader.ReadMemory<MapParentStruct>(data1.MapParentPtr);
        labels_on_ground_elem.Tick(data.itemsOnGroundLabelRoot);
        ultimatum.Tick(data.UltimatumProgressPanel, tName);
        incomin_user_request.Tick(data.incomin_user_request, tName);
        delve_darkness_elem.Tick(data.ui_debuf_panell, tName);
        modal_dialog.Tick(data.ModalDialog, tName);
        chat_box_elem.Tick(data.ChatPanel, tName);
        debuffs_pannel.Tick(data.ui_debuf_panell, tName);
        ui_ritual_rewards.Tick(data.ui_ritual_rewards, tName);
        ui_lake_map.Tick(data.ui_lake_map, tName);
        SkillBar.Tick(data.ui_skills, tName);
        ui_ppa.Tick(data.ui_passive_point_available, tName);
        ChatHelpPop.Tick(data.chat_help_pop, tName);
        ui_menu_btn.Tick(data.ui_menu_btn, tName);
        ui_xp_bar.Tick(data.ui_xp_bar, tName);
        MyBuffPanel.Tick(data.ui_buff_panel, tName);
        party_panel.Tick(data.party_panel, tName);
        GetPlayerInvetory();
    }
    void GetPlayerInvetory() { 
    
    }
    internal override void Tick(IntPtr ptr, string from) {
        Debug.Assert(ptr == default || from.EndsWith(".CleanUpData") ||from== "debug_gui");
        Address = ptr;
        Init(from);
    }
    internal UltimatumElem ultimatum { get; } = new UltimatumElem();
    internal DelveDarknessElem delve_darkness_elem { get; } = new DelveDarknessElem();
    internal ModalDialog modal_dialog { get; } = new ModalDialog() ;
    internal IncomingUserRequest incomin_user_request { get; } = new IncomingUserRequest();
    internal IList<LabelOnGround> ItemsOnGroundLabels => labels_on_ground_elem.LabelsOnGround;
    internal IList<LabelOnGround> ItemsOnGroundLabelsVisible => labels_on_ground_elem.LabelsOnGround.Where(x => x.Address != IntPtr.Zero && x.IsVisible).ToList();
    internal ItemsOnGroundLabelElement labels_on_ground_elem { get; } = new ItemsOnGroundLabelElement(IntPtr.Zero);
    internal ChatBoxElem chat_box_elem { get; } = new ChatBoxElem();
    internal SkillBarElement SkillBar { get; } = new SkillBarElement();
    internal PartyPanel party_panel { get; } = new PartyPanel();
    internal Inventory player_inventory { get; } = new Inventory(IntPtr.Zero, "player");
    internal LargeMap large_map { get; } = new(IntPtr.Zero);
    aMapElemet maps_root { get; } = new(IntPtr.Zero);
    internal StashElement stash_element { get; } = new StashElement();
    internal QuestRewardWindow QuestRewardWindow { get; } = new QuestRewardWindow();
    internal WorldMapElement world_map { get; } = new WorldMapElement();
    internal If_I_Dead if_I_dead { get; } = new If_I_Dead();
    internal Element ui_ritual_rewards { get; } = new Element("ui_ritual_rewards");
    internal Element debuffs_pannel { get; } = new Element("debuffs_pannel");
    internal Element ui_lake_map { get; } = new Element("ui_lake_map");
    internal Element ui_ppa { get; } = new Element("ui_ppa");
    internal Element ChatHelpPop { get; } = new Element("ChatHelpPop");
    internal Element ui_menu_btn { get; } = new Element("ui_menu_btn");
    internal Element ui_flask_root { get; } = new Element("ui_flask_root") ;
    internal Element ui_xp_bar { get; } = new Element("ui_xp_bar") ;
    internal Element MyBuffPanel { get; } = new Element("MyBuffPanel") ;
    public void Dispose() {
        worker.Abort();
    }
}
