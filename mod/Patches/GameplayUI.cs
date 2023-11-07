using HarmonyLib;
using Reptile;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoHUD.Patches
{
    [HarmonyPatch(typeof(GameplayUI), "Init")]
    class GameplayUI_Init_Patch
    {
        public static void Postfix(GameplayUI __instance)
        {
            if (!Core.active)
            {
                __instance.chargeBar.enabled = Core.active;
                foreach (Image img in __instance.gameObject.GetComponentsInChildren<Image>(true))
                {
                    Color c = img.color;
                    c.a = Convert.ToInt32(Core.active);
                    img.color = c;
                }

                foreach (TextMeshProUGUI text in __instance.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    Color c = text.color;
                    c.a = Convert.ToInt32(Core.active);
                    text.color = c;
                }
            }
        }
    }
}
