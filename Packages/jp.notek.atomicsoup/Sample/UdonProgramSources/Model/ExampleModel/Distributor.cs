using System;
using JP.Notek.AtomicSoup;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace AtomicSoupSample.ExampleModel
{
    [UdonSharpProgramAsset]
    [DIProvide(typeof(Value1Atom))]
    [DIProvide(typeof(Value2Atom))]
    [DIProvide(typeof(Value3Atom))]
    [DIProvide(typeof(SyncValue1Atom))]
    [DIProvide(typeof(SyncValue2Atom))]
    public class Distributor : AtomDistributor
    {
        [DIInject] public Value1Atom Value1;
        [DIInject] public Value2Atom Value2;
        [DIInject] public Value3Atom Value3;
        [DIInject] public SyncValue1Atom SyncValue1;
        [DIInject] public SyncValue2Atom SyncValue2;
        protected override DistributorConsistency Consistency => DistributorConsistency.Strong;
    }
}