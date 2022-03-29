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
        public const string PluginVersion = "1.0.0";
        public ConfigEntry<string> Equipments, AddEquipments, SetEquipmentsString;
        public static ConfigEntry<bool> Experimental, RemoveIncompatibleEquipments, SetEquipments;
        private static List<EquipmentIndex> RemoveList, AddList, ExactList;
        private static Dictionary<string, EquipmentIndex> Dict;


        public void Awake()
        {
            Log.Init(base.Logger);

            ConfigInit();
            Dict = MakeDictionary();

            //hook
            On.RoR2.EquipmentCatalog.Init += EquipmentCatalog_Init;

        }
        private void ConfigInit()
        {
            //config
            Equipments = Config.Bind("Equipment", "Remove Equipment", "Volcanic Egg", "Determines what equipment are removed from the bottled chaos pool\nrefer to the mod's webpage for a guide");

            Experimental = Config.Bind("Experimental", "Enable Experimental settings", false, "Enable experimental settings\n(will likely break something)");
            AddEquipments = Config.Bind("Experimental", "Add Equipment", "", "Determines what equipment are added to the bottled chaos pool\nrefer to the mod's webpage for a guide");
            SetEquipments = Config.Bind("Experimental", "Use Exact Pool", false, "Allows you to set an exact pool");
            SetEquipmentsString = Config.Bind("Experimental", "Pool", "0,1,5,6,8,9,10,23,24,25,26,29,32,33,37,41,44,45,48,50", "(the default value is the default pool)");
            RemoveIncompatibleEquipments = Config.Bind("Experimental", "Auto Cleanup non default equipment", false, "Enables automatic clearing for equipment that are not in the default pool\nfor example: Tricorn, Unused Equipment, Lunar Equipment");

            RemoveList = ConfigParser(Equipments);
            AddList = ConfigParser(AddEquipments);
            ExactList = ConfigParser(SetEquipmentsString);
        }

        private List<EquipmentIndex> ConfigParser(ConfigEntry<string> conf)
        {
            //Lowercase and removed spaces for consistency
            string equipments = conf.Value.ToLower();
            equipments = Regex.Replace(equipments, @"\s*", "");


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
        private static void EquipmentCatalog_Init(On.RoR2.EquipmentCatalog.orig_Init orig)
        {
            orig();

            List<EquipmentIndex> newList = EquipmentCatalog.randomTriggerEquipmentList;

            //removal
            foreach (EquipmentIndex equip in RemoveList)
            {
                newList.Remove(equip);
            }


            if (Experimental.Value)
            {
                //addition
                foreach (EquipmentIndex equip in AddList)
                {
                    newList.Add(equip);
                }

                //set exact
                if (SetEquipments.Value) newList = ExactList;

                //auto cleanup
                //all default equipments has canBeRandomlyTriggered
                if (RemoveIncompatibleEquipments.Value)
                {
                    foreach (EquipmentIndex equip in newList)
                    {
                        var equipdef = EquipmentCatalog.GetEquipmentDef(equip);
                        if (!equipdef.canBeRandomlyTriggered) newList.Remove(equip);
                    }
                }
            }
            EquipmentCatalog.randomTriggerEquipmentList = newList;
        }

        private static Dictionary<string, EquipmentIndex> MakeDictionary()
        {
            Dictionary<string, EquipmentIndex> Dict = new Dictionary<string, EquipmentIndex>();
            int c = 0;
            foreach (var Equip in EquipmentCatalog.equipmentDefs)
            {

                //Lowercase for consistency
                string InternalName = Equip.name.ToLower();
                string ExternalName = Language.GetString(Equip.nameToken).ToLower();

                //remove spaces for consistency
                InternalName = Regex.Replace(InternalName, @"\s*", "");
                ExternalName = Regex.Replace(ExternalName, @"\s*", "");

                Dict.Add($"{c}", (EquipmentIndex)c);
                Dict.Add(InternalName, (EquipmentIndex)c);
                Dict.Add(ExternalName, (EquipmentIndex)c);

                //c++ hilarious i know.
                c++;
            }

            return Dict;

        }


    }

}
