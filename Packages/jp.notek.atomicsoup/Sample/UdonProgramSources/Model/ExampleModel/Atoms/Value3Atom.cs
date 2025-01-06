using JP.Notek.AtomicSoup;
using UnityEngine;

namespace AtomicSoupSample.ExampleModel
{
    [UdonSharpProgramAsset]
    public class Value3Atom : CalculatedBoolAtom
    {
        [SerializeField, DIInject, SubscribeAtom] public Value1Atom value1;
        [SerializeField, DIInject, SubscribeAtom] public Value2Atom value2;
        protected override bool _Value { get; set; } = false;
        public override bool Factory()
        {
            return value1.Value && value2.Value;
        }
    }
}