using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoHUD.Patches
{
    [HarmonyPatch(typeof(BaseModule), "HandleStageFullyLoaded")]
    public class BaseModule_HandleStageFullyLoaded_Patch
    {
        public static void Postfix()
        {
            if (Core.isDragsunSpeedometerLoaded)
            {
                GameplayUI gui = Traverse.Create(Reptile.Core.Instance.UIManager).Field<GameplayUI>("gameplay").Value;
                Core.DragsunSpeedLabel = gui.gameplayScreen.transform.Find("Speedometer").gameObject;
                Core.DragsunSpeedLabel.SetActive(Core.active);
            }
        }
    }
}
