using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using System.Collections.Concurrent;
namespace Stas.GA;
public partial class Tests {
    public Cell test_cell;
    public V2 test_gp;
    public (V2, string) test_sp;
    public ConcurrentDictionary<V2, string> gpa = new();
    /// <summary>
    /// same Screen points for debug
    /// </summary>
    public ConcurrentDictionary<V2, string> spa = new();
    public ConcurrentBag<V2> test_gpoins = new();
    public void ToLogSame() {
        //MouseBadCentre();

        #region old
        //CameraDebug();
        //ui.AddToLog("passives_tree vis=[" + gc_ui.passives_tree.IsVisible + "]");
        //ui.AddToLog("map vis=[" + gc_ui.Map.LargeMap.IsVisible + "]");
        //Curse();
        //DebugSameSkill();
        //ui.AddToLog("can detonate=[" + ui.gc_ui.SkillBar.detonate.IsVisible + "]");
        //DebugMap();
        //TestSelectLabel();
        //ui.AddToLog("LM vis=[" + gc_ui.Map.LargeMap.IsVisible + "] b_mgui_top =[" + ui.b_mgui_top + "] b_top=["+ ui.b_top + "]", MessType.OnTop);
        //ui.AddToLog("me gpos=[" + ui.me.gpos + "]");
        //ui.AddToLog("me pos=[" + ui.me.Pos + "]");
        //var el = ui.gc.entity_list;
        //ui.AddToLog("ent=["+ el.Entities.Count + "] val=["+ ui.entities.Count + "]");
        //TestSelectLabel();
        //ui.AddToLog("passives_tree=[" + ui.gc_ui.passives_tree.IsVisible + "]");
        //ui.AddToLog("ui.b_ui_debug=[" + ui.b_ui_debug + "]");
        //ui.AddToLog("detonate=[" + ui.gc_ui.SkillBar.detonate.IsVisible + "]");
        //ui.AddToLog("b_chat_box_inp=[" + ui.b_chat_box_inp + "]");
        //ui.AddToLog("b_ui_busy=[" + ui.b_ui_busy + "]");
        //ui.AddToLog("Inv=[" + ui.gc_ui.InventoryPanel.IsVisible + "]", MessType.OnTop);
        //ui.AddToLog("InventoryPanel.IsVisible =[" + gc_ui.InventoryPanel.IsVisibleLocal + "]");
        //var stage = ui.me.Buffs.FirstOrDefault(b => b.Name == "frost_fury_stage");
        //if (stage != null) {
        //    ui.AddToLog("NumberOfStages=[" + stage.Charges + "]");
        //}
        //ui.AddToLog("detonate=[" + ui.gc_ui.SkillBar.detonate.IsVisible+ "]");
        //ui.AddToLog("selected=[" + gc_ui.NpcDialog.Continue?.b_link_seleced+"]");
        //ui.AddToLog("npc name=[" + gc_ui.NpcDialog.npc_name + "]");
        //var askill = ui.worker.rb.askill;
        //ui.AddToLog("IsOnCooldown=[" + askill.IsOnCooldown + "]");
        //ui.AddToLog("act_10=[" + ui.gc_ui.world_panel.act_10 + "]");
        //var close = ui.mapper.enemy.Where(e=>e.HasComp<Pathfinding>()).OrderBy(e => e.gdist_to_me).ToList();
        //if(close.Count > 0) {
        //    ui.AddToLog("Pathfinding=[" + close[0].GetComp<Pathfinding>().TargetMovePos.ToVector2().ToIntString() + "]");
        //}
        //ui.AddToLog("IsOnCooldown=[" + ui.worker.d5.skill.IsOnCooldown + "]");
        //ui.AddToLog("have=["+(ui.worker.d5 as aBuff).b_already_have+"]");
        //ui.AddToLog("OpenLeftPanel =[" + ui.gc_ui.OpenLeftPanel.IsVisible + "]");
        //ui.AddToLog("OpenRightPanel =[" + ui.gc_ui.OpenRightPanel.IsVisible + "]");
        //ui.AddToLog("hero play =[" + ui.gc_ui.hero_frame.play_btn?.Parent?.b_mouse_over + "]");
        //ui.AddToLog("list =[" + ui.gc_ui.hero_frame.selected?.hero_name + "]");
        //var skill =ui.worker.pull.skill;
        //ui.AddToLog("skill.IsOnCooldown =[" + skill.IsOnCooldown + "]");
        //var all = me?.GetComp<Actor>().ActorSkills;
        //var skill = all.FirstOrDefault(s => s.InternalName == "frost_bomb");
        //var id = skill.Id; //33924
        //var use_count = skill.TotalUses;
        //ui.AddToLog("b_ready =[" +ui.worker.pull.b_ready + "]"); 
        //var gdist = ui.me.gpos.GetDistance(ui.camera.curs_to_grid);
        //ui.AddToLog("gdist to cursore =[" + gdist.ToRoundStr(0) + "]");
        //ui.AddToLog("diagn =[" + ui.gc.Game.IngameState.DiagnosticInfoType + "]");
        //ui.AddToLog("NpcDialog =[" + gc_ui.NpcDialog.IsVisible+ "]");
        //ui.AddToLog("hero_bb_pos =[" + ui.camera.hero_bb_pos + "]");//+
        //AddToLog("b_moving =[" + act.isMoving + "]") ;//+
        //AddToLog("b_attaking =["+ act.isAttacking + "]" ); //+
        //var mp = Mouse.GetCursorPosition();
        //var ings = gc.Game.IngameState;
        //tasker.b_task_debug = true; //only manual task input
        //var cs = gc.Game.CurrentGameStates;
        //AddToLog("chat box inp=[" + gc_ui.chat_box_elem?.input?.IsVisible + "]");
        //OnCursorItemTooltip = gc.Game.IngameState.UIHoverTooltip.Tooltip;
        //DebugModalDialogs();
        //AddToLog("mouse pos==[" + mp.ToIntString() + "]");
        //AddToLog("cursor to tgp=[" +cursor_to_grid.ToIntString() + "]");
        //AddToLog("gpos=[" +me.gpos.ToIntString() + "]");
        //ui.AddToLog("me pos=[" +me.Pos.ToIntString() + "]");
        //ui.AddToLog("npc_dialog=[" + gc_ui.NpcDialog.IsVisible + "]"); 
        #endregion
    }
    public void FindUiElemNotUnick(string finde) { //"Mirrored Tablet" "Nessa"
        List<Element> res = new List<Element>();
        ui.gui.GetAllTextElem_by_Str(finde, res);
        if(res.Count >1)
            ui.test_elem = res[1].Parent.Parent; //8b8
    }
    public void GetTopElemUnderCursor() {//9b8
        ui.test_elem = ui.states.ingame_state.UIHover;
        //Element last_elem = null;
        //Element last_elem2 = null;
        //while (ui.test_elem.Parent != null) {
        //    last_elem2 = last_elem;
        //    last_elem = ui.test_elem;
        //    ui.test_elem = ui.test_elem.Parent;//9C8
        //}
        //ui.test_elem = last_elem2;
    }
    public void GetRootElemUnderCursor(int parent_count = 0) {//9b8
        ui.test_elem = ui.states.ingame_state.UIHover;
        var curr_parent = 0;
        while (curr_parent < parent_count) {
            if (ui.test_elem.Parent == null) {
                ui.AddToLog("Test.GEUC: cant get parent on index=" + parent_count, MessType.Error);
                return;
            }
            ui.test_elem = ui.test_elem.Parent;
            curr_parent += 1;
        }
    }
    public void uiElementFinder() {
        ui.test_elem = ui.gui.GetTextElem_by_Str("Nessa")?.Parent?.Parent;
        #region old
        //ui.test_elem = ui.gui.GetObject<Element>(0x25CA8825080).Parent.Parent; 
        //ui.test_elem = ui.gui.GetTextElem_by_Str("Unearthed Hideout").Parent.Parent?.Parent; //map=>678

        //ui.test_elem = ui.gui.GetTextElem_by_Str("Atlas Skills")?.Parent?.Parent;
        //ui.test_elem = ui.gui.GetTextElem_by_Str("Atlas")?.Parent;//610
        //ui.test_elem = ui.gui;//.WorldMap;
        //ui.test_elem = ui.gui.GetTextElem_with_Str("release")?.Parent.Parent?.Parent?.Parent;//808
        //ui.test_elem = ui.gui.GetTextElem_with_Str("Are you sure you want")?.Parent.Parent;//928
        //ui.test_elem = ui.gui.GetTextElem_by_Str()?.Parent;//648
        //ui.test_elem = ui.gui.GetTextElem_by_Str("World")?.Parent;//648
        //ui.test_elem = ui.gui.GetTextElem_by_Str("Firefly")?.Parent?.Parent; //678
        //ui.test_elem = ui.gui.GameUI;//58
        //ui.test_elem = ui.gui.GetTextElem_by_Str("ChatPanel")?.Parent;//ChatPanel=480
        //ui.test_elem = ui.gui.GetTextElem_by_Str("39x Scroll of Wisdom")?.Parent?.Parent;//itemsOnGroundLabelRoot=680
        //ui.test_elem = ui.gui.GetTextElem_with_Str("resurrect").Parent.Parent; //if_i_dead=978
        //ui.test_elem = ui.gui.GetTextElem_by_Str("Kirac Mission").Parent.Parent; //KiracMission=7F8
        //ui.test_elem = ui.gui.GetTextElem_by_Str("Mana_kola").Parent.Parent.Parent.Parent; //PartyPanel=3F0
        //ui.test_elem = ui.gui.GetTextElem_with_Str("Temple Of Atzoatl").Parent.Parent; //7C8
        //ui.test_elem = ui.gui.GetTextElemWithStr("Always Attack").Parent.Parent;
        //ui.test_elem = ui.gui.incomin_user_request;
        //ui.test_elem = ui.gui.GetTextElem_with_Str("exit to character selection");
        //ui.test_elem = ui.gui.GetElementByString("Cosmetics");
        //ui.test_elem = ui.gui.GetTextElemWithStr("Subterranean Chart").Parent.Parent;
        //ui.test_elem = ui.gc.IngameState.UIHoverTooltip;
        //ui.test_elem = ui.gui.incomin_user_request.sent_you_elem?.Parent;//+
        //ui.test_elem = ui.gui.GetTextElemWithStr("Requires Level")?.Parent; // 62(gem), 138 Str(gem),
        //ui.test_elem = ui.gui.GetTextElem_with_Str("sent you a trade request").Parent.Parent.Parent; //9F8

        //var addr =( ui.test_elem.Address + 0x160).ToString("X");
        //ui.test_elem = ui.gui.esc_dialog; //+3.17
        //ui.test_elem = ui.gui.modal_dialog; 
        #endregion

    }
}