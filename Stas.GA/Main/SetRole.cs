using System.Diagnostics;
using System.Net;

namespace Stas.GA;
public partial class ui {
    public static void SetRole() {
        switch (sett.role) {
            case Role.Master:
                tasker?.Stop("SetRole");
                udp_bot?.Dispose();
                //TODO temporary for debug old one
                tasker = new Master();
                var mname = Environment.MachineName;
                switch (mname) {
                    case "GF1030":
                        //udp_master = new UdpListener(this, IPAddress.Parse("192.168.42.53"));
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.2"));
                        break;
                    case "MANA":
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.10"));
                        break;
                    case "ONGF":
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.8"));
                        break;
                    case "DAMAGE": //54:04:A6:B1:DC:1B
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.7"));
                        break;
                    case "CURSE": //A8:5E:45:E6:5F:42
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.11"));
                        break;
                    case "LARS"://A4-50-56-3C-2B-22
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.13"));
                        break;
                    case "VOVA": //a4:50:56:2A:27:DB
                        udp_master = new UdpListener(IPAddress.Parse("192.168.1.12"));
                        break;
                    default:
                        throw new NotImplementedException(mname);
                }
                break;
            case Role.Slave:
                tasker?.Stop("SetRole");
                udp_master?.Dispose();
                udp_bot = new UdpBot();
                tasker = new Slave();
                break;
            case Role.None:
                tasker?.Stop("SetRole");
                tasker = null;
                udp_master?.Dispose();
                udp_bot?.Dispose();
                break;
            default:
                ui.AddToLog("SetRole err: not present role=[" + sett.role + "]", MessType.Critical);
                break;
        }
    }
}