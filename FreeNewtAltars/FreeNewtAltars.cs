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
        public const string PluginVersion = "1.0.0";
        private static ConfigEntry<int> NewtAltarPrice;
        public void Awake()
        {
            NewtAltarPrice = Config.Bind("", "NewtAltarPrice", 0, new ConfigDescription("Determines the price of newt altars"));
            
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            AdjustNewtPrice();
        }

        private static void AdjustNewtPrice()
        {
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
