namespace Stas.GA;
internal class WorldMapElement : Element {
    internal WorldMapElement() : base("WorldMapElement") {
    }

    internal override void Tick(IntPtr ptr, string from=null) {
        base.Tick(ptr, from);
        //Panel.Tick(ui.m.Read<IntPtr>(Address + 0xAB8, 0xC10));
        //part_1 = GetTextElem_by_Str("Part 1");
        //part_2 = GetTextElem_by_Str("Part 2");
        //epilogue = GetTextElem_by_Str("Epilogue");
        //act_10 = GetTextElem_by_Str("Act 10");
        //parts = GetChildFromIndices(2, 0, 1);
        //acts = parts?.GetChildFromIndices(1, 0, 1, 1);
        //act_10_points = acts?.GetChildFromIndices(4, 0, 2, 0);
    }
    internal Element Panel { get;  } = new Element("WME.Panel");
    internal Element part_1 { get; private set; } = new Element("WME.part_1");
    internal Element part_2 { get; private set; } = new Element("WME.part_2");
    internal Element epilogue { get; private set; } = new Element("WME.epilogue");

    internal Element act_10 { get; private set; } = new Element("WME.act_10");

    internal Element parts { get; private set; } = new Element("WME.parts");
    internal Element acts { get; private set; } = new Element("WME.acts");
    internal Element act_10_points { get; private set; } = new Element("WME.act_10_points");
} 