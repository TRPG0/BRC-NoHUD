using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;
using Reptile;
using UnityEngine.UI;
using TMPro;

namespace NoHUD
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Core : BaseUnityPlugin
    {
        private ConfigEntry<KeyCode> configToggleKey;
        internal static bool active = true;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            configToggleKey = Config.Bind("General",
                                          "ToggleKey",
                                          KeyCode.BackQuote,
                                          "The key used to toggle the HUD.");

            Harmony harmony = new Harmony("NoHUD");
            harmony.PatchAll();
        }

        private void Update()
        {
            if (Input.GetKeyDown(configToggleKey.Value) && Reptile.Core.Instance.BaseModule.IsPlayingInStage)
            {
                GameplayUI gui = Traverse.Create(Reptile.Core.Instance.UIManager).Field<GameplayUI>("gameplay").Value;
                active ^= true;

                gui.chargeBar.enabled = active;
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

                if (FindObjectOfType<GraffitiGame>())
                {
                    foreach (SpriteRenderer spr in FindObjectOfType<GraffitiGame>().GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        Color c = spr.color;
                        c.a = Convert.ToInt32(active);
                        spr.color = c;
                    }

                    foreach (TextMeshProUGUI text in FindObjectOfType<GraffitiGame>().GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        Color c = text.color;
                        c.a = Convert.ToInt32(active);
                        text.color = c;
                    }

                    foreach (LineRenderer line in FindObjectOfType<GraffitiGame>().GetComponentsInChildren<LineRenderer>(true))
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

        [HarmonyPatch(typeof(GameplayUI), "Init")]
        class GameplayUI_Init_Patch
        {
            public static void Postfix(GameplayUI __instance)
            {
                if (!Core.active)
                {
                    __instance.chargeBar.enabled = active;
                    foreach (Image img in __instance.gameObject.GetComponentsInChildren<Image>(true))
                    {
                        Color c = img.color;
                        c.a = Convert.ToInt32(active);
                        img.color = c;
                    }

                    foreach (TextMeshProUGUI text in __instance.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        Color c = text.color;
                        c.a = Convert.ToInt32(active);
                        text.color = c;
                    }
                }
            }
        }

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
}
