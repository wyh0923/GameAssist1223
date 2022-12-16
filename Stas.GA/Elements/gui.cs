using System.Diagnostics;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
    internal GameUiElements()   : base("gui") {
        worker = new Thread(() => {
            while (ui.b_running) {
                if (Address == default || ui.curr_state != gState.InGameState) {
                    Thread.Sleep(ui.w8*10);
                    continue;
                }
                base.Tick(Address, tName + ".worker_thred");
                Update();
                Thread.Sleep(100);
            }
        });
        worker.IsBackground= true;
        worker.Start(); 
    }
  
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetGuiOffsets")]
    public static extern int GetGuiOffsets(IntPtr gui_ptr, ref guiOffset offs);
   
    void Update() {
        Debug.Assert(Address != default);
        var data = new guiOffset();
        GetGuiOffsets(Address, ref data);
        map_root.Tick(data.maps_root_ptr.self, tName);
        if (map_root.children_pointers.Length == 4) {
            large_map.Tick(map_root[0].Address, tName);
        }
        map_devise.Tick(data.MapDeviceWindow, tName);
        ui_flask_root.Tick(data.ui_flask_root, tName);
        KiracMission.Tick(data.KiracMission, tName);
        open_right_panel.Tick(data.open_right_panel, tName);
        open_left_panel.Tick(data.open_left_panel, tName);
        if (children_pointers.Length > 25)
            passives_tree.Tick(children_pointers[23], tName);
        NpcDialog.Tick(data.NpcDialog, tName); //wromg insade not upd now
        LeagueNpcDialog.Tick(data.LeagueNpcDialog, tName);
        BetrayalWindow.Tick(data.BetrayalWindow, tName);
        AtlasPanel.Tick(data.AtlasPanel, tName);
        AtlasSkillPanel.Tick(data.AtlasSkillPanel, tName);
        DelveWindow.Tick(data.DelveWindow, tName);
        TempleOfAtzoatl.Tick(data.TempleOfAtzoatl, tName);
        if_I_dead.Tick(data.if_i_dead, tName);
        world_map.Tick(data.WorldMap, tName);
        stash_element.Tick(data.StashElement, tName);
        QuestRewardWindow.Tick(data.QuestRewardWindow, tName);
        labels_on_ground_elem.Tick(data.itemsOnGroundLabelRoot);
        ultimatum.Tick(data.UltimatumProgressPanel, tName);
        incomin_user_request.Tick(data.incomin_user_request, tName);
        delve_darkness_elem.Tick(data.ui_debuf_panell, tName);
        modal_dialog.Tick(data.ModalDialog, tName);
        chat_box_elem.Tick(data.ChatPanel, tName);
        debuffs_pannel.Tick(data.ui_debuf_panell, tName);
        ui_ritual_rewards.Tick(data.ui_ritual_rewards, tName);
        SkillBar.Tick(data.ui_skills, tName);
        ui_ppa.Tick(data.ui_passive_point_available, tName);
        ChatHelpPop.Tick(data.chat_help_pop, tName);
        ui_menu_btn.Tick(data.ui_menu_btn, tName);
        ui_xp_bar.Tick(data.ui_xp_bar, tName);
        MyBuffPanel.Tick(data.ui_buff_panel, tName);
        party_panel.Tick(data.party_panel, tName);
        GetEsc();
        //GetPlayerInvetory();

    }
    internal override void Tick(IntPtr ptr, string from) {
        Address = ptr;
        if (ptr == default) {//game state was cahnged to login/hero select?
            var cgs = ui.curr_state;
            Clear();
        }
    }
    protected override void Clear() {
        base.Clear();
        ui.elements.Clear();
        esc_ptr = default;
        pi_ptr = default;
        map_root.Tick(default, "Clear");
    }
    Element map_root = new Element("map_root");
    void GetLargeMap(guiOffset data) {
        map_root.Tick(data.maps_root_ptr.self, tName);
        if (map_root.children_pointers.Length == 4) {
            large_map.Tick(map_root.GetChildFromIndices(2, 0).Address, tName);
        }
        else {
            ui.AddToLog(tName + "map_root vrong children count", MessType.Error);
        }
    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetEscPtr")]
    static extern IntPtr GetEscPtr();
    IntPtr esc_ptr = default;
    void GetEsc() {
        if(esc_ptr==default)
            esc_ptr = GetEscPtr();
        if (esc_ptr != default)
            esc_dialog.Tick(esc_ptr, tName + ".GetEsc");
        else {
            ui.AddToLog(tName + ".GetEsc err=bad ptr", MessType.Error);
        }
    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPassiveTreePtr")]
    static extern IntPtr GetPassiveTreePtr();
    void GetPassiveTree() {
        var ptr = GetPassiveTreePtr();
        if (ptr != default)
            passives_tree.Tick(ptr, tName + ".GetPassiveTree");
        else {
            ui.AddToLog(tName + ".GetPassiveTree err=bad ptr", MessType.Error);
        }
    }
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetPlInvPtr")]
    static extern IntPtr GetPlInvPtr();
    IntPtr pi_ptr = default;
    void GetPlayerInvetory() {
        if(pi_ptr ==default)
            pi_ptr = GetPlInvPtr();
        if (pi_ptr != default)
            player_inventory.Tick(pi_ptr, tName + ".GetPlayerInvetory");
        else {
            ui.AddToLog(tName + ".GetPlayerInvetory err=bad ptr", MessType.Error);
        }
    }
    internal Element map_devise { get; } = new Element("map_devise");
    internal EscDialog esc_dialog { get; } = new EscDialog();
    internal Element skip_element { get; } = new Element("skip_element");
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
    internal LargeMap large_map { get; } = new LargeMap();
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
   
}
