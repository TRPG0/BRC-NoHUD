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

namespace NoHUD
{
    [BepInPlugin("NoHUD", "NoHUD", "1.1.0")]
    [BepInDependency("Speedometer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Dragsun.Speedometer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("SlopCrew.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    public class Core : BaseUnityPlugin
    {
        public ConfigEntry<KeyCode> configToggleKey;
        public ConfigEntry<bool> configSlopNames;
        internal static bool active = true;

        internal static bool isDragsunSpeedometerLoaded = false;
        internal static GameObject DragsunSpeedLabel;

        internal static bool isSoftGoatSpeedometerLoaded = false;
        internal static GameObject SoftGoatSpeedBar;

        internal static bool isSlopCrewLoaded = false;
        internal static bool SlopCrewShowNameplates
        {
            get
            {
                if (!isSlopCrewLoaded) return false;
                else return SlopCrewHelper.ShowPlayerNameplates;
            }
        }

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

            if (isSoftGoatSpeedometerLoaded)
            {
                Logger.LogInfo("Speedometer (SoftGoat) is loaded. Applying InitializeUI patch.");
                harmony.PatchAll(typeof(Speedometer_InitializeUI_Patch));
            }

            if (isDragsunSpeedometerLoaded) Logger.LogInfo("Speedometer (Dragsun) is loaded.");

            if (isSlopCrewLoaded)
            {
                Logger.LogInfo("SlopCrew is loaded. Applying SpawnNameplate patch.");
                harmony.PatchAll(typeof(AssociatedPlayer_SpawnNameplate_Patch));
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
                if (isSlopCrewLoaded && configSlopNames.Value) SlopCrewHelper.SetNameplatesActive(active);

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

    }
}
