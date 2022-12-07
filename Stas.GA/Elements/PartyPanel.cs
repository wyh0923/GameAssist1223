using System.Diagnostics;

namespace Stas.GA;

public class PartyPanel : Element {
    internal PartyPanel() : base("PartyPanel") {
    }
    //how to finde?
    //ui.test_elem = ui.gui.GetTextElemWithStr("same_party_member_name").Parent.Parent.Parent.Parent;

    public Element memb_liat_root => GetChildFromIndices(0, 0);
    List<PartyMember> _membs = new ();

   

    public List<PartyMember> members {
        get {
            _membs.Clear();
            Debug.Assert(memb_liat_root != null);
            foreach (var chld in memb_liat_root.children) {
                var nm = new PartyMember(chld.Address);
                _membs.Add(nm);
            }
            return _membs;
        }
    }
}

public class PartyMember : Element {
    public string name { get; }
    public string area_name { get; }
    public Element face_icon { get; }
    public Element portal_icon { get; }

    internal PartyMember(IntPtr address) : base(address, "PartyMember") {
        name = GetChildAtIndex(0)?.Text;
        face_icon = GetChildAtIndex(1);
        area_name = GetChildAtIndex(2)?.Text;
        portal_icon = GetChildAtIndex(3);
    }

    public override string ToString() {
        var a = "";
        if (area_name != null)
            a = "[" + area_name + "]";
        return name + a;
    }
}
