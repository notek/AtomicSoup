using JP.Notek.AtomicSoup;
using UnityEngine;

namespace AtomicSoupSample.ExampleModel
{
    [UdonSharpProgramAsset]
    public class OwnerTimeDifferenceAtom : WritableFloatAtom
    {
        protected override float _Value { get; set; } = 0;
    }
}