using System.Diagnostics;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;
public class GridCell : Cell {
    public bool b_calc_neighbour;
    public const int size = 23;
    public V2 my_pos;
    int[,] data => ui.curr_map.bit_data;
    public byte[,] checked_bit = new byte[size, size];
    bool check(int x, int y) => checked_bit[x, y] == 1;
    public int bit(int x, int y) => data[(int)min.X + x, (int)min.Y + y]; //3842 3059
    public List<GridCell> gcels_conncted = new List<GridCell>(); //only for wisual debug
    public List<Cell> blocks = new List<Cell>();
    public List<Cell> routs = new List<Cell>(); //836
    public List<Cell> border_routs = new List<Cell>();
    public float visited_persent = 0f;
    public float routs_percent;
    public float routs_weight_with_neibor;
    public TriggerableBlockage trigger;
    public bool b_trigger_corrected = false;
    public string path;
    public string tile_key;
    string _fn;
    public float routs_area { get; private set; }
    public string fname {
        get {
            if (path == null) 
                return null;
            if (_fn == null) {
                var pa = path.Split("/");
                _fn = pa[pa.Length - 1];
            }
            return _fn;
        }
    }
    public GridCell(int _id, V2 _min, V2 _max) : base(_min, _max) {
        id = _id;
        var nc = new Cell(_min.X, _min.Y, _max.X,  _max.Y) {
            id = ui.nav.lcid,
            root = this
        };
        routs.Add(nc);
    }
   
    
    public Cell Get_rout_by_gp(V2 gp) {
        return routs.Find(g => g.min.X <= gp.X && g.min.Y <= gp.Y && g.max.X >= gp.X && g.max.Y >= gp.Y);
    }
    public Cell Get_block_by_gp(V2 gp) {
        return blocks.Find(g => g.min.X <= gp.X && g.min.Y <= gp.Y && g.max.X >= gp.X && g.max.Y >= gp.Y);
    }
    public void Set_all_cell_rout() {
        if (b_trigger_corrected)
            return;
        lock (blocks) {
            foreach (var b in blocks) {
                routs.Add(b);
            }
            blocks.Clear();
            b_trigger_corrected = true;
        }
    }

  
    public bool NotFoundInOld(float x, float y) {
        foreach (var b in blocks) {
            if (b.Contains(x + min.X, y + min.Y))
                return false;
        }
        foreach (var b in routs) {
            if (b.Contains(x + min.X, y + min.Y))
                return false;
        }
        return true;
    }

   
    public void Reset() {
        checked_bit = new byte[size, size];
        routs.Clear();
        blocks.Clear();
    }
    public override string ToString() {
        return "min=" + min + " max=" + max + " id=" + id + " n=" + gcels_conncted.Count;
    }
}