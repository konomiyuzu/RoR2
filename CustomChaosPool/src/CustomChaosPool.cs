using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace CustomChaosPool
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class CustomChaosPool : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "konomiyu";
        public const string PluginName = "CustomChaosPool";
        public const string PluginVersion = "1.1.3";
        public static ConfigEntry<string> Equipments, AddEquipments, SetEquipmentsString;
        public static ConfigEntry<bool> Experimental, RemoveIncompatibleEquipments, SetEquipments, LogDebug, GiveEquipmentInfo, CapInsensitive, SpaceInsensitive, SpecialInsensitive, TricornPatch;
        private static List<EquipmentIndex> RemoveList, AddList, ExactList;
        private static Dictionary<string, EquipmentIndex> Dict;


        public void Awake()
        {
            Log.Init(base.Logger);
            ConfigInit();
            

            //hooks
            On.RoR2.EquipmentCatalog.Init += EquipmentCatalog_Init;
            IL.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;

            //ill move this into it's own mod
            //IL.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted;
        }

        

        private void ConfigInit()
        {
            //config
            CapInsensitive = Config.Bind("Selection", "Capitalization Insensitivity", true, "Enables Capitalization insensitivity\nTry disabling this if a modded equipment has the same internal name but not capitalization");
            SpaceInsensitive = Config.Bind("Selection", "Space Insensitivity", true, "Enables Space insensitivity\nTry disabling this if a modded equipment has similiar names with different spaces");
            SpecialInsensitive = Config.Bind("Selection","Special Character Insensitivity", false, "Enables Special character insensitivity\nthis includes all foreign language characters\nTry disabling this if a modded equipment consists of foreign language characters");

            Equipments = Config.Bind("Equipment", "Remove Equipment", "Volcanic Egg", "Determines what equipment are removed from the bottled chaos pool\nrefer to the mod's webpage for a guide");

            Experimental = Config.Bind("Experimental", "Enable Experimental settings", false, "Enable experimental settings\n(will likely break something)");
            AddEquipments = Config.Bind("Experimental", "Add Equipment", "", "Determines what equipment are added to the bottled chaos pool\nrefer to the mod's webpage for a guide\n(requires Experimental)");
            SetEquipments = Config.Bind("Experimental", "Use Exact Pool", false, "Allows you to set an exact pool");
            SetEquipmentsString = Config.Bind("Experimental", "Pool", "0,1,5,6,8,9,10,23,24,25,26,29,32,33,37,41,44,45,48,50", "(the default value is the default pool)\n(please note that if you have mods that adds equipment these indexes would be incorrect)\n(requires Experimental)");
            RemoveIncompatibleEquipments = Config.Bind("Experimental", "Auto Cleanup non default equipment", false, "Enables automatic clearing for equipment that are not in the default pool\nfor example: Tricorn, Unused Equipment, Lunar Equipment\n(requires Experimental)");
            TricornPatch = Config.Bind("Experimental", "Trophy Hunters Tricorn patch", false, "Enables a patch to stop your equipment from getting cosumed if trophy hunter's tricorn is in the bottled chaos pool\nmight break mods that alter tricorn\n(requires Experimental)");

            LogDebug = Config.Bind("Miscellaneous", "Log Debug", true, "Logs extra info for debugging");
            GiveEquipmentInfo = Config.Bind("Miscellaneous", "Give Equipment Info", false, "Logs the indexes, internal names and names of all equipments into the log\n(meant for use with modded equipment)");
        }

        private static List<EquipmentIndex> ConfigParser(ConfigEntry<string> conf)
        {
            //Some mods might have the same internal name with different capitilization so im adding an option for lowercase and space insensitivity
            //(and now special insensitivity)
            string equipments = conf.Value;

            if (CapInsensitive.Value) equipments = equipments.ToLower();
            if (SpaceInsensitive.Value) equipments = Regex.Replace(equipments, @"\s*", "");
            if (SpecialInsensitive.Value) equipments = Regex.Replace(equipments, @"(?![\s\w,]).", "");

            string[] arr = equipments.Split(',');
            List<EquipmentIndex> output = new List<EquipmentIndex>();
            foreach (string equip in arr)
            {
                if (Dict.ContainsKey(equip))
                {
                    output.Add(Dict[equip]);
                }
            }

            return output;
        }

        private static void SetUpLists()
        {
            Dict = MakeDictionary();
            RemoveList = ConfigParser(Equipments);
            AddList = ConfigParser(AddEquipments);
            ExactList = ConfigParser(SetEquipmentsString);
        }
        private static void EquipmentCatalog_Init(On.RoR2.EquipmentCatalog.orig_Init orig)
        {
            orig();
            string debug = "";
            SetUpLists();
            List<EquipmentIndex> newList = EquipmentCatalog.randomTriggerEquipmentList;

            //removal
            foreach (EquipmentIndex equip in RemoveList)
            {
                newList.Remove(equip);
                debug = $"{debug}\nRemoved {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})";
            }

            printdebug();

            if (Experimental.Value)
            {
                Log.LogWarning($"Experimental Settings is on");

                //addition
                foreach (EquipmentIndex equip in AddList)
                {
                    newList.Add(equip);


                    debug = $"{debug}\nAdded {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})";
                }

                printdebug();

                //set exact
                if (SetEquipments.Value) {

                    newList = ExactList;
                    Log.LogDebug($"List Set tp exact list {string.Join(",", newList)}");
                }

                //auto cleanup
                //all default equipments has canBeRandomlyTriggered
                if (RemoveIncompatibleEquipments.Value)
                {
                    foreach (EquipmentIndex equip in newList)
                    {
                        var equipdef = EquipmentCatalog.GetEquipmentDef(equip);
                        if (!equipdef.canBeRandomlyTriggered)
                        {
                            newList.Remove(equip);
                            debug = $"{debug}\nAuto Cleanup\'ed {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})";
                           
                        }

                    }

                    printdebug();
                }
            }
            EquipmentCatalog.randomTriggerEquipmentList = newList;

            Log.LogDebug($"randomTriggerEquipmentList set to {string.Join(",", EquipmentCatalog.randomTriggerEquipmentList)}");

            void printdebug()
            {
                if (debug == "") return;
                Log.LogDebug(debug);
                debug = "";
            }
        }

        private static Dictionary<string, EquipmentIndex> MakeDictionary()
        {
            string Info = "Equipment Info\nIndex : Internal Name | Name | Extra info";
            Dictionary<string, EquipmentIndex> Dict = new Dictionary<string, EquipmentIndex>();
            int c = 0;
            foreach (var Equip in EquipmentCatalog.equipmentDefs)
            {

                string InternalName = Equip.name;
                string ExternalName = Language.GetString(Equip.nameToken);

                Info = $"{Info}\n{c} : \"{InternalName}\" | \"{ExternalName}\"";

                //all default equipments have canBeRandomlyTriggered
                if(Equip.canBeRandomlyTriggered) Info = $"{Info} | Is in default pool";

                //Lowercase Option
                if (CapInsensitive.Value)
                {
                    InternalName = InternalName.ToLower();
                    ExternalName = ExternalName.ToLower();
                }


                //Remove Space option
                if (SpaceInsensitive.Value)
                {
                    InternalName = Regex.Replace(InternalName, @"\s*", "");
                    ExternalName = Regex.Replace(ExternalName, @"\s*", "");
                }

                //Remove Special option
                if (SpecialInsensitive.Value)
                {
                    InternalName = Regex.Replace(InternalName, @"(?![\s\w,]).", "");
                    ExternalName = Regex.Replace(ExternalName, @"(?![\s\w,]).", "");
                }

                //warn about duplicates
                if (Dict.ContainsKey(InternalName) || Dict.ContainsKey(ExternalName)) Log.LogWarning($"Duplicate Internal/External name for Index {c}");

                // check if != "" because a certain equipment has no name
                // and ignore duplicates as a last line of defense just in case
                // but duplicates shouldn't make it here (hopefully)
                Dict.Add($"{c}", (EquipmentIndex)c);
                if (InternalName != "" && !Dict.ContainsKey(InternalName)) Dict.Add(InternalName, (EquipmentIndex)c);
                if (ExternalName != "" && !Dict.ContainsKey(ExternalName)) Dict.Add(ExternalName, (EquipmentIndex)c);

                //c++ hilarious i know.
                c++;
            }

            if(GiveEquipmentInfo.Value)Log.LogMessage(Info);
            return Dict;

        }

        private void EquipmentSlot_FireBossHunter(ILContext il)
        {
            //patch for tricorn
            if (!TricornPatch.Value || !Experimental.Value) return;
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchDup(),
                x => x.MatchBrtrue(out _),
                x => x.MatchPop(),
                x => x.MatchLdnull(),
                x => x.MatchBr(out _),
                x => x.MatchCall<CharacterBody>("get_inventory"),
                x => x.MatchCall<UnityEngine.Object>("op_Implicit"),
                x => x.MatchBrfalse(out _)
                );
            c.Index += 8;
            c.RemoveRange(17);
            c.Emit(OpCodes.Ldarg_0);

            //Adds characterbody to the stack
            c.Emit(OpCodes.Call, typeof(EquipmentSlot).GetMethod("get_characterBody"));
            c.EmitDelegate<Action<CharacterBody>>((cb) =>
            {

                if (cb.inventory.currentEquipmentIndex != EquipmentCatalog.FindEquipmentIndex("BossHunter")) return;

                //original code
                CharacterMasterNotificationQueue.PushEquipmentTransformNotification(cb.master,
                cb.inventory.currentEquipmentIndex, DLC1Content.Equipment.BossHunterConsumed.equipmentIndex,
                CharacterMasterNotificationQueue.TransformationType.Default);
                cb.inventory.SetEquipmentIndex(DLC1Content.Equipment.BossHunterConsumed.equipmentIndex);
            });

            Log.LogWarning("Tricorn Patch Applied");
        }
    }


}