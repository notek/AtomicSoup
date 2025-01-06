
using JP.Notek.AtomicSoup;
using UdonSharp;
using AtomicSoupSample.ExampleModel;
namespace AtomicSoupSample
{
    [DIProvide(typeof(Distributor)), UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AtomicSoupSampleModule : UdonSharpBehaviour
    { }
}