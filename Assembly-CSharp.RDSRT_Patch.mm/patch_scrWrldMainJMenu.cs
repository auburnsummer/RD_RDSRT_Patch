using System;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::scrWrldMainMenu")]
    public class patch_scrWrldMainMenu : scrWrldMainMenu
    {
        public extern void orig_SelectedCreditsOption();
        public void SelectedCreditsOption() 
        {
            if (((patch_scrWrldGame)scrWrldGame.instance).CurrentRole == RDSRT_Role.Sender)
            {
                Debug.Log("ROLE IS NOW RECEIVER");
                ((patch_scrWrldGame)scrWrldGame.instance).CurrentRole = RDSRT_Role.Receiver;
            }
            else
            {
                Debug.Log("ROLE IS NOW SENDER");
                ((patch_scrWrldGame)scrWrldGame.instance).CurrentRole = RDSRT_Role.Sender;
            }
        }   


    }
}
