using HarmonyLib;
using Reptile;
using System;
using TMPro;
using UnityEngine;

namespace NoHUD.Patches
{
    [HarmonyPatch(typeof(GraffitiGame), "Init")]
    class GraffitiGame_Init_Patch
    {
        public static void Postfix(GraffitiGame __instance)
        {
            if (!Core.active)
            {
                foreach (SpriteRenderer spr in __instance.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    Color c = spr.color;
                    c.a = Convert.ToInt32(Core.active);
                    spr.color = c;
                }

                foreach (TextMeshProUGUI text in __instance.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    Color c = text.color;
                    c.a = Convert.ToInt32(Core.active);
                    text.color = c;
                }

                foreach (LineRenderer line in __instance.GetComponentsInChildren<LineRenderer>(true))
                {
                    if (Core.active)
                    {
                        line.startWidth = 0.2491f;
                        line.endWidth = 0.2491f;
                    }
                    else
                    {
                        line.startWidth = 0;
                        line.endWidth = 0;
                    }
                }
            }
        }
    }
}
