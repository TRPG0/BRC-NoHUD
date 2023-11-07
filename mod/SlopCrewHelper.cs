using SlopCrew.Plugin;
using SlopCrew.Plugin.UI;

namespace NoHUD
{
    internal static class SlopCrewHelper
    {
        public static bool ShowPlayerNameplates => Plugin.SlopConfig.ShowPlayerNameplates.Value;

        public static void SetNameplatesActive(bool active)
        {
            if (!ShowPlayerNameplates) return;

            foreach (AssociatedPlayer player in Plugin.PlayerManager.AssociatedPlayers)
            {
                if (player.SlopPlayer.Name == Plugin.SlopConfig.Username.Value) continue;

                player.ReptilePlayer.interactionCollider.transform.GetChild(0).GetChild(0).gameObject.SetActive(active);
            }
        }
    }
}
