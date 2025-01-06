
using System;
using JP.Notek.AtomicSoup;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace AtomicSoupSample
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ExampleObjectAdapter : AtomSubscriberForView
    {
        [SerializeField, DIInject] public ExampleModel.Value1Atom _Value1;
        [SerializeField, DIInject] public ExampleModel.Value2Atom _Value2;
        [SerializeField, DIInject, SubscribeAtom] public ExampleModel.Value3Atom _Value3;

        public override void OnChange()
        {
            Debug.Log($"value1={_Value1.Value} value2={_Value2.Value} value3={_Value3.Value}");

            //更新が終わるまで参照をロックしたい場合
            // if (SyncRequestId == -1)
            //     Debug.Log($"value1={_Value1.Value} value2={_Value2.Value} value3={_Value3.Value}");
            // else
            //     Debug.Log("Locked");
        }
    }
}