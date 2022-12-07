using V2 = System.Numerics.Vector2;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Numerics;

namespace Stas.GA {
    public class NavRes {
        public readonly Stack<PathInfo> path;
        public uint hash;
        public V2 tgp { get; private set; }
        public GridCell gs;
        public Cell cell;
        /// <summary>
        /// path length in grid units
        /// </summary>
        public float distance;//path grid lenngth;
        public bool b_err => distance == 0 || path.Count < 2;
        /// <summary>
        /// waypoint we have already passed
        /// </summary>
        public HashSet<V2> visited = new ();
        public NavRes(V2 _tgp) {
            hash = ui.curr_map_hash;
            path = new Stack<PathInfo>();
            tgp = _tgp;
            distance = 0;
        }

        public int gdist_to_me(Entity ent) {
            return gdist_to_me(ent.gpos);
        }

        public int gdist_to_me(V2 tgp) {
            var gc = ui.nav.Get_gc_by_gp(tgp);
            Debug.Assert(gc != null);
            var conv = path.ToList();
            Debug.Assert(hash == ui.curr_map_hash);
            var res = 0f;
            var current = conv[0];
            foreach(var pi in conv) {
                if(pi.g_cell.min == gc.min && pi.g_cell.max == gc.max)
                    break;
                else {
                    res += current.v2.GetDistance(pi.v2);
                    current = pi;
                }
            }
            if(res == 0) { //not found gc i need in vurrent path

            }
            return (int)res;

        }
    }
   
}
