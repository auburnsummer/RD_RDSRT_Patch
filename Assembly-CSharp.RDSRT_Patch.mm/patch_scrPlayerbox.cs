using System;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::scrPlayerbox")]
    public class patch_scrPlayerbox : scrPlayerbox
    {
        public extern void orig_SpaceBarEvent(bool CPUTriggered = false);
        public new void SpaceBarEvent(bool CPUTriggered) {
            Debug.Log(String.Format("[Playerbox] Press! Time: {0} Frame: {1}", Utils.CurrentTimePassed(), Time.frameCount));
            orig_SpaceBarEvent(CPUTriggered);
        }

        public extern void orig_SpaceBarReleased(RDPlayer player, bool CPUTriggered = false);
        public new void SpaceBarReleased(RDPlayer player,bool CPUTriggered = false)
        {
            Debug.Log(String.Format("[Playerbox] Release! Time: {0} Frame: {1}", Utils.CurrentTimePassed(), Time.frameCount));
            orig_SpaceBarReleased(player, CPUTriggered);
        }

    }
}
