using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;
using Reptile;
using UnityEngine.UI;
using TMPro;
using BepInEx.Bootstrap;
using NoHUD.Patches;
using SlopCrew.API;
using System.Collections.Generic;

namespace NoHUD
{
    [BepInPlugin("NoHUD", "NoHUD", "1.1.0")]
    [BepInDependency("Speedometer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Dragsun.Speedometer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("SlopCrew.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    public class Core : BaseUnityPlugin
    {
        public static ConfigEntry<KeyCode> configToggleKey;
        public static ConfigEntry<bool> configSlopNames;
        internal static bool active = true;

        internal static bool isDragsunSpeedometerLoaded = false;
        internal static GameObject DragsunSpeedLabel;

        internal static bool isSoftGoatSpeedometerLoaded = false;
        internal static GameObject SoftGoatSpeedBar;

        internal static bool isSlopCrewLoaded = false;

        private void Awake()
        {
            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loaded.");

            configToggleKey = Config.Bind("General",
                "ToggleKey",
                KeyCode.BackQuote,
                "The key used to toggle the HUD.");

            configSlopNames = Config.Bind("General",
                "SlopNames",
                true,
                "Sets whether the names of other players in SlopCrew will be affected or not. Has no effect if SlopCrew is not loaded.");

            Harmony harmony = new Harmony("NoHUD");
            harmony.PatchAll(typeof(BaseModule_HandleStageFullyLoaded_Patch));
            harmony.PatchAll(typeof(GameplayUI_Init_Patch));
            harmony.PatchAll(typeof(GraffitiGame_Init_Patch));

            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID == "Speedometer") isSoftGoatSpeedometerLoaded = true;
                else if (plugin.Value.Metadata.GUID == "com.Dragsun.Speedometer") isDragsunSpeedometerLoaded = true;
                else if (plugin.Value.Metadata.GUID == "SlopCrew.Plugin") isSlopCrewLoaded = true;
            }

            if (isSoftGoatSpeedometerLoaded) Logger.LogInfo("Speedometer (SoftGoat) is loaded.");
            if (isDragsunSpeedometerLoaded) Logger.LogInfo("Speedometer (Dragsun) is loaded.");
            if (isSlopCrewLoaded)
            {
                Logger.LogInfo("SlopCrew is loaded.");
                var api = APIManager.API;
                api.OnPlayerCountChanged += (api) =>
                {
                    FindSlopCrewNames();
                };
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(configToggleKey.Value) && Reptile.Core.Instance.BaseModule.IsPlayingInStage && !Reptile.Core.Instance.BaseModule.IsLoading)
            {
                GameplayUI gui = Traverse.Create(Reptile.Core.Instance.UIManager).Field<GameplayUI>("gameplay").Value;
                active ^= true;

                gui.chargeBar.enabled = active;
                if (SoftGoatSpeedBar != null) SoftGoatSpeedBar.SetActive(active);
                if (DragsunSpeedLabel != null) DragsunSpeedLabel.SetActive(active);

                foreach (Image img in gui.gameObject.GetComponentsInChildren<Image>(true))
                {
                    Color c = img.color;
                    c.a = Convert.ToInt32(active);
                    img.color = c;
                }

                foreach (TextMeshProUGUI text in gui.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    Color c = text.color;
                    c.a = Convert.ToInt32(active);
                    text.color = c;
                }

                GraffitiGame gg = FindObjectOfType<GraffitiGame>();
                if (gg != null)
                {
                    foreach (SpriteRenderer spr in gg.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        Color c = spr.color;
                        c.a = Convert.ToInt32(active);
                        spr.color = c;
                    }

                    foreach (TextMeshProUGUI text in gg.GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        Color c = text.color;
                        c.a = Convert.ToInt32(active);
                        text.color = c;
                    }

                    foreach (LineRenderer line in gg.GetComponentsInChildren<LineRenderer>(true))
                    {
                        if (active)
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

        public static void FindSlopCrewNames()
        {
            if (!Reptile.Core.Instance.BaseModule.IsPlayingInStage || Reptile.Core.Instance.BaseModule.IsLoading || !isSlopCrewLoaded || !configSlopNames.Value) return;
            foreach (Player player in FindObjectsOfType<Player>())
            {
                try
                {
                    player.transform.Find("RootObject").Find("interactionCapsule").Find("SlopCrew_NameplateContainer").gameObject.SetActive(active);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
