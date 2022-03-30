using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CustomChaosPool
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class CustomChaosPool : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "konomiyu";
        public const string PluginName = "CustomChaosPool";
        public const string PluginVersion = "1.0.3";
        public static ConfigEntry<string> Equipments, AddEquipments, SetEquipmentsString;
        public static ConfigEntry<bool> Experimental, RemoveIncompatibleEquipments, SetEquipments, LogDebug, GiveEquipmentInfo, CapInsensitive, SpaceInsensitive;
        private static List<EquipmentIndex> RemoveList, AddList, ExactList;
        private static Dictionary<string, EquipmentIndex> Dict;


        public void Awake()
        {
            Log.Init(base.Logger);

            ConfigInit();
            

            //hook
            On.RoR2.EquipmentCatalog.Init += EquipmentCatalog_Init;

        }
        private void ConfigInit()
        {
            //config
            CapInsensitive = Config.Bind("Selection", "Capitalization Insensitivity", true, "Enables Capitalization insensitivity\nTry disabling this if a modded equipment has the same internal name but not capitalization");
            SpaceInsensitive = Config.Bind("Selection", "Space Insensitivity", true, "Enables Space insensitivity\nTry disabling this if a modded equipment has similiar names with different spaces");

            Equipments = Config.Bind("Equipment", "Remove Equipment", "Volcanic Egg", "Determines what equipment are removed from the bottled chaos pool\nrefer to the mod's webpage for a guide");

            Experimental = Config.Bind("Experimental", "Enable Experimental settings", false, "Enable experimental settings\n(will likely break something)");
            AddEquipments = Config.Bind("Experimental", "Add Equipment", "", "Determines what equipment are added to the bottled chaos pool\nrefer to the mod's webpage for a guide\n(requires Experimental)");
            SetEquipments = Config.Bind("Experimental", "Use Exact Pool", false, "Allows you to set an exact pool");
            SetEquipmentsString = Config.Bind("Experimental", "Pool", "0,1,5,6,8,9,10,23,24,25,26,29,32,33,37,41,44,45,48,50", "(the default value is the default pool)\n(please note that if you have mods that adds equipment these indexes would be incorrect)\n(requires Experimental)");
            RemoveIncompatibleEquipments = Config.Bind("Experimental", "Auto Cleanup non default equipment", false, "Enables automatic clearing for equipment that are not in the default pool\nfor example: Tricorn, Unused Equipment, Lunar Equipment\n(requires Experimental)");

            LogDebug = Config.Bind("Miscellaneous", "Log Debug", true, "Logs extra info for debugging");
            GiveEquipmentInfo = Config.Bind("Miscellaneous", "Give Equipment Info", false, "Logs the indexes, internal names and names of all equipments into the log\n(meant for use with modded equipment)");
        }

        private static List<EquipmentIndex> ConfigParser(ConfigEntry<string> conf)
        {
            //Some mods might have the same internal name with different capitilization so im adding an option for lowercase and space insensitivity
            string equipments = conf.Value;

            if(CapInsensitive.Value) equipments = equipments.ToLower();
            if (SpaceInsensitive.Value) equipments = Regex.Replace(equipments, @"\s*", "");


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
            string debug = "\n";
            SetUpLists();
            List<EquipmentIndex> newList = EquipmentCatalog.randomTriggerEquipmentList;

            //removal
            foreach (EquipmentIndex equip in RemoveList)
            {
                newList.Remove(equip);
                debug = $"{debug}Removed {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})\n";
            }

            printdebug();

            if (Experimental.Value)
            {
                Log.LogWarning($"Experimental Settings is on");

                //addition
                foreach (EquipmentIndex equip in AddList)
                {
                    newList.Add(equip);


                    debug = $"{debug}Added {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})\n";
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
                            debug = $"{debug}Auto Cleanup'ed {equip} ({Language.GetString(EquipmentCatalog.GetEquipmentDef(equip).nameToken)})\n";
                           
                        }

                    }

                    printdebug();
                }
            }
            EquipmentCatalog.randomTriggerEquipmentList = newList;

            Log.LogDebug($"randomTriggerEquipmentList set to {string.Join(",", EquipmentCatalog.randomTriggerEquipmentList)}");

            void printdebug()
            {
                if (debug == "\n") return;
                Log.LogDebug(debug);
                debug = "\n";
            }
        }

        private static Dictionary<string, EquipmentIndex> MakeDictionary()
        {
            string Info = "Equipment Info\nIndex : Internal Name | Name\n";
            Dictionary<string, EquipmentIndex> Dict = new Dictionary<string, EquipmentIndex>();
            int c = 0;
            foreach (var Equip in EquipmentCatalog.equipmentDefs)
            {

                string InternalName = Equip.name;
                string ExternalName = Language.GetString(Equip.nameToken);

                Info = $"{Info}{c} : {InternalName} | {ExternalName}\n";

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

                //warn about duplicates
                if (Dict.ContainsKey(InternalName) || Dict.ContainsKey(ExternalName)) Log.LogWarning($"Duplicate Internal/External name for Index {c}");

                // check if != "" because a certain equipment has no internal name
                // and ignore duplicates as a last line of defense just in case
                // but duplicates shouldn't make it here (hopefully)
                Dict.Add($"{c}", (EquipmentIndex)c);
                if (InternalName != "" && !Dict.ContainsKey(InternalName)) Dict.Add(InternalName, (EquipmentIndex)c);
                if (ExternalName != "" && !Dict.ContainsKey(ExternalName)) Dict.Add(ExternalName, (EquipmentIndex)c);

                //c++ hilarious i know.
                c++;
            }

            if(GiveEquipmentInfo.Value)Log.LogInfo(Info);
            return Dict;

        }


    }

}