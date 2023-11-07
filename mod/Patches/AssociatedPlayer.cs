using HarmonyLib;
using SlopCrew.Plugin;

namespace NoHUD.Patches
{
    [HarmonyPatch(typeof(AssociatedPlayer), "SpawnNameplate")]
    public class AssociatedPlayer_SpawnNameplate_Patch
    {
        public static void Postfix(AssociatedPlayer __instance)
        {
            if (!Core.active)
            {
                __instance.ReptilePlayer.interactionCollider.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
