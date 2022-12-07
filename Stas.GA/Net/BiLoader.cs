using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace Stas.GA {
    public partial class BotInfo {
        public byte[] ToByte() {
            using (var ms = new System.IO.MemoryStream()) {
                using (var bw = new BinaryWriter(ms)) {
                    bw.Write(b_portal_close.ToByte());
                    bw.Write(have_error.ToByte());
                    bw.Write(use_chest.ToByte());
                    bw.Write(use_loot.ToByte());
                    bw.Write(b_debug.ToByte());
                    bw.Write(b_i_died.ToByte());
                    bw.Write(loading_map.ToByte());
                    bw.Write(flares);
                    bw.Write(tnt);
                    bw.Write(pos.ToByte());
                    bw.Write(lgdist);
                    bw.Write(danger);
                    bw.Write(chest);
                    bw.Write(loot);
                    bw.Write(quests);
                    bw.Write(cpu);
                    bw.Write(mem);
                    bw.Write(map_hash);
                    bw.Write(tgp.ToByte());
                    bw.Write(state.To_UTF8_Byte());
                    bw.Write(bot_name.To_UTF8_Byte());
                    bw.Write(host_name.To_UTF8_Byte());
                    bw.Write(map_name.To_UTF8_Byte());
                }
                return ms.ToArray();
            }
        }
    }
}
