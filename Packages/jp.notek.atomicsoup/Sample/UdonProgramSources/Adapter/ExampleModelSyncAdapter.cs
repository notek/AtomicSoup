
using JP.Notek.AtomicSoup;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace AtomicSoupSample
{
    public class ExampleModelSyncAdapter : AtomSubscriberForAtom
    {
        //TODO: いい感じにライブラリ側でカバーする
        [DIInject, SubscribeAtom] public ExampleModel.SyncValue1Atom SyncValue1Atom;
        [DIInject, SubscribeAtom] public ExampleModel.SyncValue2Atom SyncValue2Atom;
        [DIInject] public ExampleModel.OwnerTimeDifferenceAtom OwnerTimeDifferenceAtom;
        [UdonSynced] public bool SyncValue1 = false;
        [UdonSynced] public bool SyncValue2 = false;
        public bool Dirty { get; private set; } = false;
        public bool Ready { get; private set; } = false;

        public override void OnChange()
        {
            Dirty = true;
            TrySync();
        }

        public void TrySync()
        {
            if (!GetIsOwner())
                TakeOwnership();
            SyncValue1 = SyncValue1Atom.Value;
            SyncValue2 = SyncValue2Atom.Value;
            RequestSerialization();
        }

        public override void OnPostSerialization(SerializationResult serializationResult)
        {
            if (serializationResult.success)
            {
                OwnerTimeDifferenceAtom.Set(0);
                Dirty = false;
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
            SyncValue1Atom.Set(SyncValue1);
            SyncValue2Atom.Set(SyncValue2);
            OwnerTimeDifferenceAtom.Set(Time.realtimeSinceStartup - deserializationResult.sendTime);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (GetIsOwner())
                RequestSerialization();
        }

        public bool GetIsOwner()
        {
            return Networking.IsOwner(Networking.LocalPlayer, gameObject);
        }

        public void TakeOwnership()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
}