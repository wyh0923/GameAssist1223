namespace Stas.GA
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     All offsets over here are UiElements.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct guiOffset {
        [FieldOffset(0x058)] public IntPtr ui_game; //3.19.2b
        [FieldOffset(0x7F8)] public IntPtr KiracMission; //3.19
        [FieldOffset(0x928)] public IntPtr ModalDialog;// are u sure, resurect
        [FieldOffset(0x978)] public IntPtr if_i_dead;//3.19.1
        [FieldOffset(0x7C8)] public IntPtr TempleOfAtzoatl; //3.19.1
        [FieldOffset(0x3F0)] public IntPtr party_panel; //3.19.1 
        [FieldOffset(0x5C0)] public IntPtr UltimatumStart;
        [FieldOffset(0x908)] public IntPtr ArchNemesis;
        [FieldOffset(0x948)] public IntPtr MinionsPanel;
        [FieldOffset(0x9C8)] public IntPtr ExpedPlacement; //3.19.1
        [FieldOffset(0x048)] public IntPtr ui_buff_panel;
        [FieldOffset(0x050)] public IntPtr ui_debuf_panell;
        [FieldOffset(0x3C8)] public IntPtr chat_help_pop;
        [FieldOffset(0x2E8)] public IntPtr ui_menu_btn;
        [FieldOffset(0x458)] public IntPtr ui_passive_point_available;
        [FieldOffset(0x2C8)] public IntPtr ui_flask_root;
        [FieldOffset(0x2D0)] public IntPtr ui_xp_bar;
        [FieldOffset(0x9A8)] public IntPtr ui_lake_map; 
        [FieldOffset(0x9C0)] public IntPtr ui_ritual_rewards;
        [FieldOffset(0xAC8)] public IntPtr incomin_user_request; 
        [FieldOffset(0x710)] public IntPtr NpcDialog;              
        [FieldOffset(0x718)] public IntPtr LeagueNpcDialog;        
        [FieldOffset(0x8C0)] public IntPtr MirroredTablet; 
        [FieldOffset(0x650)] public IntPtr WorldMap;                       
        [FieldOffset(0x680)] public IntPtr itemsOnGroundLabelRoot; 
        [FieldOffset(0x628)] public IntPtr CharacterPanel;                 
        [FieldOffset(0x630)] public IntPtr OptionPanel;    
        [FieldOffset(0x568)] public IntPtr InventoryPanel; 
        [FieldOffset(0x660)] public IntPtr Decoration;
        [FieldOffset(0x548)] public IntPtr open_left_panel; 
        [FieldOffset(0x550)] public IntPtr open_right_panel;
        [FieldOffset(0x570)] public IntPtr StashElement;                     
        [FieldOffset(0x578)] public IntPtr GuildStashElement;                
        [FieldOffset(0x618)] public IntPtr AtlasPanel;                     
        [FieldOffset(0x620)] public IntPtr AtlasSkillPanel;                
        [FieldOffset(0x480)] public IntPtr ChatPanel;                       
        [FieldOffset(0x3E0)] public IntPtr ui_skills;
        [FieldOffset(0x808)] public IntPtr BetrayalWindow;
        [FieldOffset(0x678)] public IntPtr maps_root_ptr; 
        [FieldOffset(0x6B8)] public IntPtr no_ui_here; 
        [FieldOffset(0x748)] public IntPtr QuestRewardWindow;
        [FieldOffset(0x750)] public IntPtr PurchaseWindow;
        [FieldOffset(0x758)] public IntPtr ExpeditionPurchaseWindow;
        [FieldOffset(0x770)] public IntPtr SellWindow;
        [FieldOffset(0x768)] public IntPtr ExpeditionSellWindow;
        [FieldOffset(0x778)] public IntPtr TradeWindow;
        [FieldOffset(0x780)] public IntPtr LabyrinthDivineFontPanel;
        [FieldOffset(0x7A0)] public IntPtr MapDeviceWindow;
        [FieldOffset(0x7F8)] public IntPtr IncursionWindow;
        [FieldOffset(0x818)] public IntPtr DelveWindow;
        [FieldOffset(0x828)] public IntPtr ZanaMissionChoice;
        [FieldOffset(0x848)] public IntPtr CraftBenchWindow;
        [FieldOffset(0x850)] public IntPtr UnveilWindow;
        [FieldOffset(0x878)] public IntPtr SynthesisWindow;
        [FieldOffset(0x890)] public IntPtr MetamorphWindow;
        [FieldOffset(0x8A0)] public IntPtr HarvestWindow;
        [FieldOffset(0x8A8)] public IntPtr HeistContractPanel;
        [FieldOffset(0x8B0)] public IntPtr HeistRevealPanel;
        [FieldOffset(0x8B8)] public IntPtr HeistAllyEquipmentPanel;
        [FieldOffset(0x8C0)] public IntPtr HeistBlueprintPanel;
        [FieldOffset(0x8C8)] public IntPtr HeistLockerPanel;
        [FieldOffset(0x8D0)] public IntPtr RitualWindow;
        [FieldOffset(0x8D8)] public IntPtr RitualFavourPanel;
        [FieldOffset(0x8E0)] public IntPtr UltimatumProgressPanel;
        [FieldOffset(0x988)] public IntPtr AreaInstanceUi;
        [FieldOffset(0xA80)] public IntPtr InvitesPanel;
        [FieldOffset(0xAC8)] public IntPtr GemLvlUpPanel;
        [FieldOffset(0xBB0)] public IntPtr ItemOnGroundTooltip;
        [FieldOffset(0xA80)] public IntPtr MapTabWindowStartPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MapParentStruct
    {
        [FieldOffset(0x280)] public IntPtr LargeMapPtr;
        [FieldOffset(0x288)] public IntPtr MiniMapPtr;
    }
}
