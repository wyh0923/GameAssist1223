namespace Stas.GA;

public class QuestRewardWindow : Element {
    internal QuestRewardWindow() : base("QuestRewardWindow") {
    }
    Element PossibleRewardsWrapper;
    public IList<Element> PossibleRewards { get; private set; }
    public Element CancelButton { get; private set; }
    public Element SelectOneRewardString { get; private set; }

    internal override void Tick(IntPtr ptr, string from = null) {
        base.Tick(ptr, from);
        //PossibleRewardsWrapper = GetChildFromIndices(5, 0, 0);
        PossibleRewards = PossibleRewardsWrapper?.children;
        CancelButton = GetChildAtIndex(3);
        SelectOneRewardString = GetChildAtIndex(0);
    }

}
