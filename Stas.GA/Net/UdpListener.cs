using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using System.Linq;


namespace Stas.GA {
    public class UdpListener :IDisposable {
        string tname =>this.GetType().Name;
        public BotInfo bi;
        //ManualResetEvent mre_mem = new ManualResetEvent(false);
        Stopwatch sw;
        /// <summary>
        /// active screen center
        /// </summary>
        Vector2 sc {
            get {
                var rech = EXT.GetActiveWindowRectangle();
                return new Vector2(rech.Width / 2 +rech.Left, rech.Height / 2+rech.Top);
            }
        }
        Socket srv_socket;
        bool was_disposed = false;
        public UdpListener(IPAddress ip) {
            sw = new Stopwatch();
            sw.Restart();
            srv_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var srv_ep = new IPEndPoint(ip, ui.sett.master_port); 
            srv_socket.Bind(srv_ep);
            ui.AddToLog("UDP_listener started at: "+ ip + ":"+ ui.sett.master_port);
            EndPoint bep = new IPEndPoint(IPAddress.Any, 0); //bot next end point
            var data = new byte[1024];
            string GetString() {
                return Encoding.UTF8.GetString(data, 3, BitConverter.ToInt16(data, 1));
            }
            var thread_reading = new Thread(() => {
                while(was_disposed) {
                    try {
                        var count = srv_socket.ReceiveFrom(data, ref bep); 
                        var opc = (Opcode)data[0];
                        var bip = (IPEndPoint)bep; //bot IP
                        
                        var bot = ui.bots.FirstOrDefault(b => 
                                    b.ip.ToString() == bip.Address.ToString() && b.port == bip.Port);
                        if(bot == null) {
                            bot = new BotInfo(bip.Address, bip.Port) { socket = srv_socket };
                            ui.bots.Add(bot);
                        }
                        switch(opc) {
                            case Opcode.PlaySound: {
                                    var fname = GetString();
                                    ui.sound_player.PlaySound(fname);
                                    bot.log.Add(bot.bot_name+" play: "+fname);
                                }
                                break;
                            case Opcode.Log: {
                                    var new_log = GetString();
                                    bot.log.Add(new_log);
                                }
                                break;
                            case Opcode.BotInfo:
                                bot.FillFromByteArray(data, 1); //first byte is opcode
                                bot.last_ping_time = DateTime.Now;
                                break;
                            case Opcode.BotRoleList:
                                //var ra = JSON.FromByte<List<string>>(pack.data, 1);
                                //ui.SetBotRoles(ra);
                                break;
                            case Opcode.Message: {
                                    var mess = GetString();
                                    var okstrings = new string[] { "@From" };
                                    var ok = false;
                                    foreach(var b in okstrings) {
                                        if(mess.Contains(b)) {
                                            ok = true;
                                            break;
                                        }
                                    }
                                    if(!ok)
                                        break;
                                    ui.AddToLog(tname + ":" + mess);
                                }
                                break;
                            case Opcode.OpenInBrowser: {
                                    var link = GetString();
                                    ui.AddToLog(tname + ": navigate to link: " + link);
                                    var ps = new ProcessStartInfo(link) {
                                        UseShellExecute = true,
                                        Verb = "open"
                                    };
                                    System.Diagnostics.Process.Start(ps);
                                }
                                break;
                            case Opcode.Server_down:
                                break;
                            case Opcode.Ping:
                                break;
                            case Opcode.Unknown:
                            default:
                                ui.AddToLog(tname + " Err: Unknown opcode=" + opc);
                                break;
                        }
                    }
                    catch(Exception ex) {
                        ui.AddToLog(tname + " Err: " + ex.Message);
                        Thread.Sleep(5000);
                    }
                }
            });
            thread_reading.IsBackground = true;
            thread_reading.Start();
        }
        
        public void Dispose() {
            try {
                was_disposed = true;
                srv_socket?.Shutdown(SocketShutdown.Both);
                srv_socket?.Close();
                srv_socket?.Dispose();
                srv_socket = null;
            } catch (Exception ex) {
                ui.AddToLog(this.GetType().Name + " err: " + ex.Message, MessType.Error);
            }
        }
    }
}
