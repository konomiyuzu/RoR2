using BepInEx;
using RoR2;
using BepInEx.Configuration;
//using System.Collections.Generic;

using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ShowBottledChaosEquipment
{
    [BepInPlugin(ModInfo.PluginGUID, ModInfo.PluginName, ModInfo.PluginVersion)]
    public class ShowBottledChaosEquipment : BaseUnityPlugin
    {
        public ConfigEntry<int> notifDisableAfterX;
        public ConfigEntry<float> notiftime, MinimumTime;
        public ConfigEntry<bool> Proportionaltime;
        public void Awake()
        {
            Log.Init(base.Logger);
            SetupConfig();
            IL.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted;
        }

        private void SetupConfig()
        {
            notiftime = Config.Bind("Notification", "Notification Length", 3f, "Determines the notification length in seconds");
            Proportionaltime = Config.Bind("Notification", "Proportional mode", true, "If enabled, mod will adjust length of notifications proportional to your bottled chaos count");
            MinimumTime = Config.Bind("Notification", "Minimum Length", 0f, "Determines the minimum notification length in seconds");
            notifDisableAfterX = Config.Bind("Notification", "Disable after x", 5, "when to disable the mod\nenter a negative value for never");
        }
        private void EquipmentSlot_OnEquipmentExecuted(ILContext il)
        {

            ILCursor c = new ILCursor(il);

            //its stupidly long because 
            // "generics are nasty"
            // - Bubbet#2639
            c.GotoNext(
                x => x.MatchCallvirt(out _),
                x => x.MatchBlt(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdcI4(out _),
                x => x.MatchStloc(out _),
                x => x.MatchBr(out _),
                x => x.MatchLdcI4(out _),
                x => x.MatchStloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchCallvirt(out _),
                x => x.MatchRem(),
                x => x.MatchStloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchCallvirt(out _),
                x => x.MatchStloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(out _),
                x => x.MatchAdd(),
                x => x.MatchStloc(out _)
                );
            c.Index += 22;
            c.Emit(OpCodes.Ldarg_0);

            //push characterbody onto the stack
            c.Emit(OpCodes.Call, typeof(EquipmentSlot).GetMethod("get_characterBody"));

            //push index onto the stack
            c.Emit(OpCodes.Ldloc, 9);

            c.EmitDelegate<Action<CharacterBody, EquipmentIndex>>((cb, index) =>
            {
                int bottlecount = cb.inventory.GetItemCount(DLC1Content.Items.RandomEquipmentTrigger);

                //disable feature
                if (bottlecount >= notifDisableAfterX.Value && !(notifDisableAfterX.Value < 0)) return;

                //setup for notif stuff
                var notifqueue = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(cb.master);
                var notifinfo = new CharacterMasterNotificationQueue.NotificationInfo(EquipmentCatalog.GetEquipmentDef(index), null);
                if (!notifqueue || notifinfo == null) return;

                //time stuff
                float time = notiftime.Value;
                if (Proportionaltime.Value) time /= bottlecount;
                if (time < MinimumTime.Value) time = MinimumTime.Value;

                //i dont understand this either, i just copied half the code from CharacterMasterNotificationQueue.PushEquipmentNotification()
                notifqueue.PushNotification(notifinfo, time);

            });

            Log.LogDebug(il.ToString());
        }
    }
}