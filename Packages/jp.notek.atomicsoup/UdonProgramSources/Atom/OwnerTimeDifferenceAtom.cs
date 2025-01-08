using JP.Notek.AtomicSoup;
using UnityEngine;

namespace JP.Notek.AtomicSoup
{
    [UdonSharpProgramAsset]
    public class OwnerTimeDifferenceAtom : WritableFloatAtom
    {
        protected override float _Value { get; set; } = 0;
    }
}