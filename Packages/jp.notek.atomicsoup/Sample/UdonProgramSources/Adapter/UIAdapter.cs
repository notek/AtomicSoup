using System;
using System.Collections.Generic;
using JP.Notek.AtomicSoup;
using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace AtomicSoupSample
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UIAdapter : UdonSharpBehaviour
    {
        [SerializeField, DIInject] ExampleModel.Value1Atom _Value1;
        [SerializeField, DIInject] ExampleModel.Value2Atom _Value2;
        [SerializeField] TextMeshProUGUI _LogText;

        public void OnButtonEvent1x1Pressed()
        {
            _LogText.text = $"OnButtonEvent1x1Pressed\n{_LogText.text}";
            _Value1.Set(!_Value1.Value);
        }
        public void OnButtonEvent2x1Pressed()
        {
            _LogText.text = $"OnButtonEvent2x1Pressed\n{_LogText.text}";
            _Value2.Set(!_Value2.Value);
        }
        public void OnButtonEvent3x1Pressed()
        {
            _LogText.text = $"OnButtonEvent3x1Pressed\n{_LogText.text}";
        }
        public void OnButtonEventClearPressed()
        {
            _LogText.text = $"OnButtonEventClearPressed\n{_LogText.text}";
        }
        public void OnButtonEventMixPressed()
        {
            _LogText.text = $"OnButtonEventMixPressed\n{_LogText.text}";
            for(int i=0;i<100;i++){
            }

        }

    }
}
