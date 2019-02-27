using System;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::scrWrldGame")]
    public class patch_scrWrldGame : scrWrldGame
    {
        public RDSRT_Role CurrentRole = RDSRT_Role.Receiver;

        public int P1BitfieldLastFrame = 0;
        public int P2BitfieldLastFrame = 0;

        public extern void orig_Update();
        public new void Update()
        {
            if (CurrentRole == RDSRT_Role.Sender)
            {
                // Compare last inputs to these inputs, if they've changed write to the file.
                int P1BitfieldThisFrame = Utils.InputBitfield_P1;
                int P2BitfieldThisFrame = Utils.InputBitfield_P2;

                if (P1BitfieldLastFrame != P1BitfieldThisFrame && this.receptive && !this.paused)
                {
                    P1BitfieldLastFrame = P1BitfieldThisFrame;
                    Utils.WriteEvent(Utils.GetCurrentSongName(), Utils.CurrentDonteTime(false), 0, P1BitfieldThisFrame);
                }

                if (P2BitfieldLastFrame != P2BitfieldThisFrame && this.receptive && !this.paused)
                {
                    P2BitfieldLastFrame = P2BitfieldThisFrame;
                    Utils.WriteEvent(Utils.GetCurrentSongName(), Utils.CurrentDonteTime(true), 1, P2BitfieldThisFrame);
                }
            }
            else if (CurrentRole == RDSRT_Role.Receiver)
            {

            }
            orig_Update();
        }
    }
}
