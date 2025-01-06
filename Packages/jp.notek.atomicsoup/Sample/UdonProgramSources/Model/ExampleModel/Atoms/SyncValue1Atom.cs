using JP.Notek.AtomicSoup;
namespace AtomicSoupSample.ExampleModel
{
    [UdonSharpProgramAsset]
    public class SyncValue1Atom : WritableBoolAtom
    {
        protected override bool _Value { get; set; } = false;
    }
}