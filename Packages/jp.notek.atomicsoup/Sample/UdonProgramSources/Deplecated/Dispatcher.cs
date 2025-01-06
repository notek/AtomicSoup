using System;
using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace JP.Notek.AtomicSoup
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Dispatcher : UdonSharpBehaviour
    {
        [SerializeField] int _DefaultQueueLength = 32;

        // 値変更の配布をスキップ可能にする
        [SerializeField] bool _OmitDistribution = true;

        // _OmitDistribution == trueのとき適用される設定
        // 現在のfpsがこの数字より低い場合、イベント処理を抑える。
        [SerializeField] float _MinFPS = 15;
        // Reduceの処理時間が下記より長い場合、イベント処理を抑える。
        [SerializeField] float _UpdateMaxPeriodMS = 8;

        IReduceStore[] _Stores = new IReduceStore[] { };
        string[] _QueueAction = new string[32];
        bool[] _QueueHasValue = new bool[32];
        DataToken[] _QueueValue = new DataToken[32];
        bool[] _VRCUrlQueueHasValue = new bool[32];
        VRCUrl[] _VRCUrlQueueValue = new VRCUrl[32];
        int _QueueLength = 32;
        int _QueueReadHead = 0;
        int _QueueWriteHead = 0;

        void Awake()
        {
            InitQueue(_DefaultQueueLength);
        }

        void Update()
        {
            if (GetIsStateDistributing())
                return;

            var period = Time.realtimeSinceStartup;

            bool reducing = true;
            bool changed = false;
            while (reducing)
            {
                var queueHasValue = _QueueHasValue[_QueueReadHead];
                var vRCUrlQueueHasValue = _VRCUrlQueueHasValue[_QueueReadHead];

                if (!queueHasValue && !vRCUrlQueueHasValue)
                    break;

                if (queueHasValue)
                {
                    foreach (var store in _Stores)
                    {
                        store.Reduce(_QueueAction[_QueueReadHead], _QueueValue[_QueueReadHead]);
                        changed = true;
                    }
                    _QueueHasValue[_QueueReadHead] = false;
                }
                else if (vRCUrlQueueHasValue)
                {
                    foreach (var store in _Stores)
                    {
                        store.Reduce(_QueueAction[_QueueReadHead], _VRCUrlQueueValue[_QueueReadHead]);
                        changed = true;
                    }
                    _VRCUrlQueueHasValue[_QueueReadHead] = false;
                }
                var nextHead = (_QueueReadHead + 1) % _QueueLength;
                if (_OmitDistribution)
                {
                    if (Time.deltaTime > 1f / _MinFPS)
                    {
                        reducing = false;
                    }
                    else
                    {
                        if ((Time.realtimeSinceStartup - period) > (_UpdateMaxPeriodMS / 1000f))
                        {
                            reducing = false;
                        }
                        else
                        {
                            reducing = _QueueHasValue[nextHead] || _VRCUrlQueueHasValue[nextHead];
                        }
                    }
                }
                else
                {
                    reducing = false;
                }
                _QueueReadHead = nextHead;
            }
            if(changed)
                SetIsStateDistributing();
        }
        public void RegisterStore(IReduceStore store)
        {
            _Stores = _Stores.Add(store);
        }

        public void Dispatch(string action, DataToken value = new DataToken())
        {
            AddQueue(action, value);
        }

        public void Dispatch(string action, VRCUrl url)
        {
            AddQueue(action, url);
        }

        void AddQueue(string action, DataToken value)
        {
            if (_QueueHasValue[_QueueWriteHead] || _VRCUrlQueueHasValue[_QueueWriteHead])
            {
                RescaleQueue();
            }
            _QueueAction[_QueueWriteHead] = action;
            _QueueHasValue[_QueueWriteHead] = true;
            _QueueValue[_QueueWriteHead] = value;
            _QueueWriteHead = (_QueueWriteHead + 1) % _QueueLength;
        }

        void AddQueue(string action, VRCUrl value)
        {
            if (_QueueHasValue[_QueueWriteHead] || _VRCUrlQueueHasValue[_QueueWriteHead])
            {
                RescaleQueue();
            }
            _QueueAction[_QueueWriteHead] = action;
            _VRCUrlQueueHasValue[_QueueWriteHead] = true;
            _VRCUrlQueueValue[_QueueWriteHead] = value;
            _QueueWriteHead = (_QueueWriteHead + 1) % _QueueLength;
        }

        void InitQueue(int queueLength)
        {
            _QueueHasValue = new bool[queueLength];
            _QueueAction = new string[queueLength];
            _QueueValue = new DataToken[queueLength];
            _VRCUrlQueueHasValue = new bool[queueLength];
            _VRCUrlQueueValue = new VRCUrl[queueLength];

            _QueueLength = queueLength;
        }

        void RescaleQueue()
        {
            int queueLength = _QueueLength * 2;

            string[] queueAction = new string[queueLength];
            bool[] queueHasValue = new bool[queueLength];
            DataToken[] queueValue = new DataToken[queueLength];
            bool[] vRCUrlQueueHasValue = new bool[queueLength];
            VRCUrl[] vRCUrlQueueValue = new VRCUrl[queueLength];

            Array.Copy(_QueueAction, 0, queueAction, 0, _QueueWriteHead);
            Array.Copy(_QueueAction, _QueueWriteHead, queueAction, _QueueWriteHead + _QueueLength, _QueueLength - _QueueWriteHead);
            Array.Copy(_QueueHasValue, 0, queueHasValue, 0, _QueueWriteHead);
            Array.Copy(_QueueHasValue, _QueueWriteHead, queueHasValue, _QueueWriteHead + _QueueLength, _QueueLength - _QueueWriteHead);
            Array.Copy(_QueueValue, 0, queueValue, 0, _QueueWriteHead);
            Array.Copy(_QueueValue, _QueueWriteHead, queueValue, _QueueWriteHead + _QueueLength, _QueueLength - _QueueWriteHead);
            Array.Copy(_VRCUrlQueueHasValue, 0, vRCUrlQueueHasValue, 0, _QueueWriteHead);
            Array.Copy(_VRCUrlQueueHasValue, _QueueWriteHead, vRCUrlQueueHasValue, _QueueWriteHead + _QueueLength, _QueueLength - _QueueWriteHead);
            Array.Copy(_VRCUrlQueueValue, 0, vRCUrlQueueValue, 0, _QueueWriteHead);
            Array.Copy(_VRCUrlQueueValue, _QueueWriteHead, vRCUrlQueueValue, _QueueWriteHead + _QueueLength, _QueueLength - _QueueWriteHead);

            _QueueHasValue = queueHasValue;
            _QueueAction = queueAction;
            _QueueValue = queueValue;
            _VRCUrlQueueHasValue = vRCUrlQueueHasValue;
            _VRCUrlQueueValue = vRCUrlQueueValue;

            if (_QueueReadHead >= _QueueWriteHead)
                _QueueReadHead += _QueueLength;
            _QueueLength = queueLength;
        }

        bool GetIsStateDistributing()
        {
            foreach (var store in _Stores)
            {
                if (store.IsStateDistributing)
                    return true;
            }
            return false;
        }

        void SetIsStateDistributing()
        {
            foreach (var store in _Stores)
            {
                store.IsStateDistributing = true;
            }
        }
    }
}