using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Stas.GA {
    public struct PathInfo {
        public GridCell g_cell;
        public Cell cell;
        public Vector2 v2;
        public PathInfo(GridCell _gc, Cell _c, Vector2 _v2) {
            g_cell = _gc;
            cell = _c;
            v2 = _v2;
        }
        public override string ToString() {
            return v2.X+" "+v2.Y;
        }
    }
}
