using System;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::RDBase")]
    public class patch_RDBase : RDBase
    {
        // I guess printe got compiled out in our preorder version?
        // but it's probably useful to see it for modding purposes
        public extern void orig_printe(object msg);
        public new void printe(object msg)
        {
            Debug.Log(msg);
        }

    }
}
