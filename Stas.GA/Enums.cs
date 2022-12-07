using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stas.GA {
    public enum Axis {
        X,
        Y
    }
    [Flags]
    public enum Role {
        None = 0,
        Slave = 1,
        Master = 2,//pull manual or auto
        AiBot = 4,//AI auto farm
        Coollector = 8,
        //Trader = 16 //now separated application on phone
    }
    public enum TaskErrors {
        NoError, NavRes_no_path, im_stuck_here, i_m_dead, init_error, ent_error,
        TimeOut,
        Cant_get_Top,
        SetTop,
        skill_not_ready,
        cant_save_hit,
        Cant_see_tgp,
        tgp_is_zero
    }
    public enum Opcode : byte {
        Unknown,
        Ping,
        Message,
        LoginOK,
        Server_down,
        ServerReady,
        MouseLeftClick, //чтоююот мог лутать и заходить в двери
        MouseRightClick,
        MouseLeftDown,
        MouseLeftUp,
        KeyDown,
        KeyUp,
        KeyPress,
        ShiftDown,
        ShiftUp,
        BotRoleList,
        SetRole,
        StartMoving,
        StopMoving,
        SavingAss,
        StopAll,
        UseFlare,
        UseTNT,
        OpenInBrowser,
        StartScreening,
        StopScreening,
        OneImageBuffer,
        ImageOver,
        Test,
        ScreenQ,
        MouseScroll,
        OpenTrade,
        Looting,
        GetPos,
        SetTarget,
        NewImage,
        BotInfo,
        CleareErr,
        ReloadMem,
        UpdateMap,
        NewMapItemsList,
        ItemsList,
        RestartUdp,
        Transit,
        SetLeader,
        Log,
        Hold,
        UseLoot,
        UseChest,
        UnHold,
        TpToLeader,
        Jump,
        SetCursorOnScreen,
        NewImagePart,
        ResetState,
        SetUltim,
        FocusGP,
        FallowHard,
        Exit,
        UseTotem,
        ResurectCheckPOint,
        ResurectTown,
        NavGo,
        PlaySound,
        LinkMe,
        //TpToHeist
    }


    public enum UserType : byte { PC, Android, Ios }
    
  
}