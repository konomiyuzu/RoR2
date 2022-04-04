using BepInEx;
using BepInEx.Configuration;
using RoR2;
//using UnityEngine;
//using System.Collections;

namespace FreeNewtAltars
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class FreeNewtAltars : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "konomiyu";
        public const string PluginName = "FreeNewtAltars";
        public const string PluginVersion = "1.1.0";
        private static ConfigEntry<int> NewtAltarPrice, MinimumStageCount;
        private static ConfigEntry<bool> StageMode, After;
        public void Awake()
        {

            //cleanup old
            Config.Bind("", "NewtAltarPrice", 0, new ConfigDescription("Determines the price of newt altars"));
            Config.Remove(new ConfigDefinition("", "NewtAltarPrice"));

            NewtAltarPrice = Config.Bind("Price", "NewtAltarPrice", 0, new ConfigDescription("Determines the price of newt altars"));

            StageMode = Config.Bind("Stage", "UseStage", false, new ConfigDescription("Determines if the mod is active before/after a set stage"));
            MinimumStageCount = Config.Bind("Stage", "Stage", 5);
            After = Config.Bind("Stage", "Before/After", true, new ConfigDescription("false = before\ntrue = after"));


            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            AdjustNewtPrice();
        }

        private static void AdjustNewtPrice()
        {
            int StageCount = Run.instance.stageClearCount + 1;
            //if stage mode is on
            if (StageMode.Value && (

                // if current stage is <= stagecount and after is true
                //this is for consistency with before, as in before the value is toggled at MinimumStageCount + 1
                //without the "=" it would be toggled at MinimumStageCount
                (StageCount <= MinimumStageCount.Value && After.Value) ||

                // if current stage is > stagecount and after is false
                (StageCount > MinimumStageCount.Value && After.Value == false)
                )) return;

            var purchaseInteractions = InstanceTracker.GetInstancesList<PurchaseInteraction>();
            foreach (PurchaseInteraction purchaseInteraction in purchaseInteractions)
            {
                if (purchaseInteraction.name.StartsWith("NewtStatue"))
                {
                    
                    purchaseInteraction.cost = NewtAltarPrice.Value;
                    if (NewtAltarPrice.Value == 0) purchaseInteraction.costType = CostTypeIndex.None;
                }
            }

        }
    }
}
