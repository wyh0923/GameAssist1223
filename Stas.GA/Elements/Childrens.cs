using System.Text;
using V2 = System.Numerics.Vector2;
namespace Stas.GA;

public partial class Element : RemoteObjectBase {
    long _last_ch_hash;
    List<Element> _childrens = new();
    public List<Element> children = new();
    public IntPtr[] children_pointers { get; private set; }
    IntPtr first_children_offset = 0x30;
    public void GetChildren(string from) {
        var ch_ptr = ui.m.Read<StdVector>(Address + first_children_offset);
        if (ch_ptr.First == 0 || ch_ptr.End == 0 || chld_count < 0) {
            return;
        }
        var curr_hash = ch_ptr.GetHashCode();
        if (curr_hash == _last_ch_hash)
            return;

        _childrens.Clear();
        children_pointers = ui.m.ReadStdVector<IntPtr>(ch_ptr);
        var chi = 0;

        foreach (var _ptr in children_pointers) {
            var ch = new Element(_ptr, tName + ".child[" + chi + "]");
            ch.Tick(ch.Address, from);
            _childrens.Add(ch);
            chi += 1;
        }
        _last_ch_hash = curr_hash;

        children = _childrens;
    }
    public Element GetChildFromIndices(params int[] indices) {
        var curr_elem = this;

        StringBuilder BuildErrorString(int errorIndex) {
            var str = new StringBuilder();
            foreach (var i in indices) {
                if (i == errorIndex) {
                    str.Append('>');
                }

                str.AppendFormat("[{0}] ", i);
                if (i == errorIndex) {
                    str.Append('<');
                }
            }

            return str;
        }

        for (var indexNumber = 0; indexNumber < indices.Length; indexNumber++) {
            var index = indices[indexNumber];
            curr_elem = curr_elem.GetChildAtIndex(index);
            if (curr_elem == null) {
                ui.AddToLog($"{tName} with index {index} was not found. Indices: " +
                    $"{BuildErrorString(indexNumber)}", MessType.Error);
                return null;
            }

            if (curr_elem.Address == IntPtr.Zero) {
                ui.AddToLog($"{tName} with index {index} has address = 0. Indices: " +
                    $"{BuildErrorString(indexNumber)}", MessType.Error);
                return null;
            }
        }

        return curr_elem;
    }
    public Element GetTextElem_with_Str(string str, bool only_vis = true, bool ignore_case = true) {
        if (only_vis && !this.IsVisible)
            return null;

        if (ignore_case) {
            if (Text != null && Text.ToLower().Contains(str, StringComparison.OrdinalIgnoreCase)) {
                return this;
            }
        }
        else {
            if (Text != null && Text.Contains(str)) {
                return this;
            }
        }

        GetChildren("GetTextElem_with_Str");
        foreach (var ch in children) {
            var element = ch.GetTextElem_with_Str(str);
            if (element != null) return element;
        }
        return null;
    }
    public Element GetTextElem_by_Str(string str, bool only_vis = true, bool ignore_case = true) {
        var curr = tName;
        var test = str;
        if (ignore_case) {
            if (Text?.ToLower() == str.ToLower()) {
                return this;
            }
        }
        else {
            if (Text == str) {
                return this;
            }
        }
        GetChildren("GetTextElem_by_Str");
        foreach (var ch in children) {
            var element = ch.GetTextElem_by_Str(str);
            if (element != null)
                return element;
        }
        return null;
    }
    public void GetAllTextElem_by_Str(string str, List<Element> res) {
        if (Text?.ToLower() == str.ToLower()) {
            res.Add(this);
        }
        GetChildren("GetAllTextElem_by_Str");
        foreach (var ch in children)
            ch.GetAllTextElem_by_Str(str, res);
    }
    public Element GetElem_ends_wit(string str, bool ignore_case = true) {
        if (ignore_case) {
            if (Text != null && Text.ToLower().EndsWith(str.ToLower())) {
                return this;
            }
        }
        else {
            if (Text != null && Text.EndsWith(str)) {
                return this;
            }
        }
        GetChildren("GetElem_ends_wit");
        foreach (var ch in children) {
            var element = ch.GetElem_ends_wit(str);
            if (element != null)
                return element;
        }
        return null;
    }
}
