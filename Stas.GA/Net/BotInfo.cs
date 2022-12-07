#region using
using System.Numerics;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using V2 = System.Numerics.Vector2;
using V3 = System.Numerics.Vector3;
using System.Diagnostics;
#endregion

namespace Stas.GA {
    public partial class BotInfo  {
        public Socket socket;
        public readonly IPAddress ip;
        public readonly int port;
        public bool b_show_log = false;
        public bool b_i_died = false;
        public bool b_portal_close = false;
        public Entity ent;
        public V2 tgp; //current bot target grid pos
        EndPoint ep;
        public BotInfo(IPAddress _ip, int _port) {
            ip = _ip;
            port = _port;
            ep = new IPEndPoint(ip, port);
        }
        public void Send(Opcode opc, string _str) {
            Send(opc, _str.To_UTF8_Byte());
        }

        public void Send(string _str) {
            Send(Opcode.Message, _str.To_UTF8_Byte());
        }
        public void Send(Opcode opc) {
            try {
                if (socket == null) {
                    ui.AddToLog(this.GetType().Name + " socket err...", MessType.Error);
                    return;
                }
                socket.SendTo(new byte[] { (byte)opc }, 1, SocketFlags.None, ep);
            } catch (Exception ex) {
                ui.AddToLog(this.GetType().Name + " err: " + ex.Message, MessType.Error);
            }
        }
        public void Send(Opcode opc, byte[] ba) {
            try {
                if (socket == null) {
                    ui.AddToLog(this.GetType().Name + " socket err...", MessType.Error);
                    return;
                }
                var fba = BYTE.Concat(new byte[] { (byte)opc }, ba);
                socket.SendTo(fba, fba.Length, SocketFlags.None, ep);
            } catch (Exception ex) {
                ui.AddToLog(this.GetType().Name + " err: " + ex.Message, MessType.Error);
            }
        }
        public FixedSizedLog log  = new FixedSizedLog(30);
        public DateTime last_ping_time; //this need only on master side
        public bool have_error;
        public bool use_chest;
        public bool use_loot;
        public bool b_debug;
        public bool loading_map;
        public byte flares;
        public byte tnt;
        public int chest;
        public int loot;
        public int quests;
        public float cpu;
        public float mem;
        public string state;
        public string bot_name;
        public uint map_hash;
        /// <summary>
        /// bot greed pos
        /// </summary>
        public V3 pos;
        /// <summary>
        /// distance to the Leader in Grid point
        /// </summary>
        public float lgdist;
        /// <summary>
        /// last known the Target gpos
        /// </summary>
        public V2 last_tgpos;
        public float danger;
        public string map_name = "None";
        public string host_name;
       
        public void FillFromByteArray(byte[] ba, int offs = 0) { // офссет на опкод
            Debug.Assert(offs < 5);
            using (var ms = new MemoryStream(ba)) {
                ms.Position = offs;
                using (var br = new BinaryReader(ms)) {
                    b_portal_close = br.ReadByte().ToBool();
                    have_error = br.ReadByte().ToBool();
                    use_chest = br.ReadByte().ToBool();
                    use_loot = br.ReadByte().ToBool();
                    b_debug = br.ReadByte().ToBool();
                    b_i_died = br.ReadByte().ToBool();
                    loading_map = br.ReadByte().ToBool();
                    flares = br.ReadByte();
                    tnt = br.ReadByte();
                    pos = br.ReadBytes(12).ToSysNumV3();
                    lgdist = br.ReadSingle();
                    danger = br.ReadSingle();
                    chest = br.ReadInt32();
                    loot = br.ReadInt32();
                    quests = br.ReadInt32();
                    cpu = br.ReadSingle();
                    mem = br.ReadSingle();
                    map_hash = br.ReadUInt32();
                    tgp = br.ReadBytes(8).ToSysNumV2();
                    state = br.To_UTF8_String();
                    bot_name = br.To_UTF8_String();
                    host_name = br.To_UTF8_String();
                    map_name = br.To_UTF8_String();
                }
            }
        }
        public override string ToString() {
            return "n="+ bot_name +" hn="+ host_name;
        }
    }
}
