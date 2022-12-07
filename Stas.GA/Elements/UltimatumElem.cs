namespace Stas.GA; 

public class UltimatumElem : Element {
    internal UltimatumElem() : base("UltimatumElem") {
    }

    public Element accept => GetTextElem_by_Str("accept trial");
    public Element confirm => GetTextElem_by_Str("confirm");
    public int selected_choice {
        get {
            var ea = GetChildFromIndices(2, 4, 0);
            for (int i = 0; i < ea.children.Count; i++) {
                if (ea.children[i].b_selected) {
                    return i;
                }
            }
            return -1;
        }
    }
}
