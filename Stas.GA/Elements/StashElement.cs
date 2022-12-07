namespace Stas.GA;
internal class StashElement : Element {
    internal StashElement() : base("StashElement") {
    }

    public long TotalStashes { get; private set; }
    public Element StashInventoryPanel { get; } = new Element("StashInventoryPanel");
    public Element ViewAllStashButton { get; } = new Element("ViewAllStashButton");
    public Element ViewAllStashPanel { get; } = new Element("ViewAllStashPanel");

    internal override void Tick(IntPtr ptr, string from=null) {
        //StashInventoryPanel.Address = M.Read<IntPtr>(Address + 0x2F8, 0x280, 0x980);
        //ViewAllStashButton.Address = M.Read<IntPtr>(Address + 0x2F8, 0x280, 0x988);//err 3.19
        //ViewAllStashPanel.Address = M.Read<IntPtr>(Address + 0x2F8, 0x280, 0x990); //err 3.19
        TotalStashes = StashInventoryPanel?.chld_count ?? 0;
    }
   
    public string GetStashName(int index) {
        if (index >= TotalStashes || index < 0) {
            return string.Empty;
        }
        var viewAllStashPanelChildren = this.ViewAllStashPanelChildren;
        Element element;
        if (viewAllStashPanelChildren == null) {
            element = null;
        }
        else {
            var element2 = viewAllStashPanelChildren.ElementAt(index);
            IList<Element> children = element2.GetChildAtIndex(0).children;
            if (element2 == null) {
                element = null;
            }
            else {
                element = children?.Last();
            }
        }
        return element == null ? string.Empty : element.Text;
    }
    public Inventory GetStashInventoryByIndex(int index) //This one is correct
    {
        if (index >= TotalStashes) return null;
        if (index < 0) return null;
        if (StashInventoryPanel.children[index].chld_count == 0) return null;

        Inventory stashInventoryByIndex = null;

        try {
            var inv_ptr = StashInventoryPanel.children[index].children[0].children[0].Address;
            stashInventoryByIndex = new Inventory(inv_ptr, "?");
        }
        catch {
            ui.AddToLog($"Not found inventory stash for index: {index}", MessType.Error);
        }

        return stashInventoryByIndex;
    }
    public IList<Element> ViewAllStashPanelChildren {
        get {
            Element viewAllStashPanel = ViewAllStashPanel;
            if (viewAllStashPanel == null) {
                return null;
            }
            return viewAllStashPanel.children.Last(x => x.chld_count == TotalStashes).children.Where((Element x) => {
                IList<Element> children = x.children;
                return children != null && children.Count > 0;
            }).ToList();
        }
    }
}