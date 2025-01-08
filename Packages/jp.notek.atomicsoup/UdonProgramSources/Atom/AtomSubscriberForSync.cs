using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace JP.Notek.AtomicSoup
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public abstract class AtomSubscriberForSync : AtomSubscriber
    {
        [SerializeField, DIInject] protected OwnerTimeDifferenceAtom _OwnerTimeDifferenceAtom;
        public bool Ready { get; private set; } = false;

        protected abstract void ReflectAtomToUdonSynced();
        protected abstract void ReflectUdonSyncedToAtom();
        public override void OnChange()
        {
            TrySync();
        }

        public bool GetIsOwner()
        {
            return Networking.IsOwner(Networking.LocalPlayer, gameObject);
        }

        public override void OnPostSerialization(SerializationResult serializationResult)
        {
            if (serializationResult.success)
            {
                _OwnerTimeDifferenceAtom.Set(0);
            }
            else
            {
                TrySync();
            }
        }

        public override void OnDeserialization(DeserializationResult deserializationResult)
        {
            if (!Ready && !deserializationResult.isFromStorage)
                Ready = true;
            ReflectUdonSyncedToAtom();
            _OwnerTimeDifferenceAtom.Set(Time.realtimeSinceStartup - deserializationResult.sendTime);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (GetIsOwner())
                RequestSerialization();
        }

        void TrySync()
        {
            if (!GetIsOwner())
                TakeOwnership();
            ReflectAtomToUdonSynced();
            RequestSerialization();
        }

        void TakeOwnership()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
}