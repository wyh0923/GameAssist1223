#region using
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion
namespace Stas.GA {
    public class UdpSound {
        string tname => this.GetType().Name;
        Socket socket;
        byte[] data = new byte[1024];
        int b_count;
        bool b_need_remake = true;

        internal void Send(object playSound) {
            ui.AddToLog(tname+ "Send err: debug me", MessType.Error);
        }

        public UdpSound() {
            var thred = new Thread(() => {
                while (true) {
                    try {
                        if (b_need_remake) {
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            socket.Connect(IPAddress.Parse(ui.sett.master_IP), ui.sett.master_port);
                            b_count = socket.Receive(data, 0); //last packet
                            var opc = (Opcode)data[0];
                            switch (opc) {
                                case Opcode.Ping:
                                    break;
                                default:
                                   ui.AddToLog("UdpSound opc=" + opc, MessType.Error);
                                   break;
                            }
                        }
                    } catch (Exception ex) {
                        ui.AddToLog(tname + "... " + ex.Message, MessType.Error);
                        socket.Dispose();
                        b_need_remake = true;
                        Thread.Sleep(5000);
                    }
                    Thread.Sleep(3);
                }
            });
            thred.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            thred.IsBackground = true;
            //thred.Start();
        }
        public void Send(Opcode opc, byte[] data) {
            var ba = BYTE.Concat(new byte[] { (byte)opc }, data);
            try {
                socket?.Send(ba);
            } catch (Exception ex) {
                ui.AddToLog(tname + "=>Send... " + ex.Message);
            }
        }

        public void Dispose() {
            try {
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                socket?.Dispose();
                socket = null;

            } catch (Exception ex) {
                ui.AddToLog(this.GetType().Name + " err: " + ex.Message, MessType.Error);
            }
        }
    }
}
