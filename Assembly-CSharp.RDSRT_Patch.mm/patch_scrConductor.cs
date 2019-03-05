using System;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::scrConductor")]
    public class patch_scrConductor : scrConductor
    {
        [MonoModPublic]
        public string currentSongString;

        public double audioPosThisFrame;
        public int lastFrameUpdated = 0;

        public System.Random rng;

        public extern void orig_Awake();
        public void Awake()
        {
            rng = new System.Random();
            orig_Awake();
        }

        public extern double orig_get_audioPos();
        public double get_audioPos()
        {
            // if fake news is active, return whatever we say the time is.
            if (Utils.FakeNews)
            {
                return Utils.FakeNewsValue;
            }
            // only return a single audioPos per frame, so that each PlayerBox operates on the same time.
            if (Time.frameCount != lastFrameUpdated)
            {
                audioPosThisFrame = orig_get_audioPos();
                lastFrameUpdated = Time.frameCount;
            }
            return audioPosThisFrame;
        }
    }
}
