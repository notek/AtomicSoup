
using System;
using JP.Notek.AtomicSoup;
using TMPro;
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
        [SerializeField, DIInject, SubscribeAtom] public ExampleModel.Value1Atom _Value1;
        [SerializeField, DIInject, SubscribeAtom] public ExampleModel.Value2Atom _Value2;
        [SerializeField, DIInject, SubscribeAtom] public ExampleModel.Value3Atom _Value3;
        [SerializeField, DIInject, SubscribeAtom] public ExampleModel.SyncValue1Atom _SyncValue1;
        [SerializeField] TextMeshProUGUI _LogText;

        public override void OnChange()
        {
            _LogText.text = $"value1={_Value1.Value} value2={_Value2.Value} value3={_Value3.Value}\n{_LogText.text}";

            //更新が終わるまで参照をロックしたい場合
            if (!_SyncValue1.IsDirty)
                _LogText.text = $"SyncValue1={_SyncValue1.Value}\n{_LogText.text}";
            else
                _LogText.text = $"SyncValue1 is updating\n{_LogText.text}";
        }
    }
}