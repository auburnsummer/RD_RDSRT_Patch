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
    }
}
