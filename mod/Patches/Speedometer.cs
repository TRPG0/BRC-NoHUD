using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Speedometer;

namespace NoHUD.Patches
{
    [HarmonyPatch(typeof(Plugin), "InitializeUI")]
    public class Speedometer_InitializeUI_Patch
    {
        public static void Postfix(Transform speedBarBackground, Image speedBar, TextMeshProUGUI tricksLabel)
        {
            Core.SoftGoatSpeedBar = speedBar.gameObject;
            speedBar.gameObject.SetActive(Core.active);
        }
    }
}
