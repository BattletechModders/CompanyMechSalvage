using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using BattleTech;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace CompanyMechSalvage
{
    [HarmonyPatch(typeof(Contract), "GenerateSalvage")]
    public static class Contract_GenerateSalvage_Patch
    {
        static void Postfix(Contract __instance, List<UnitResult> enemyMechs, List<VehicleDef> enemyVehicles, List<UnitResult> lostUnits, bool logResults = false)
        {
            Logger.LogLine("Postfix Start");
            Settings settings = CompanyMechSalvage.LoadSettings();
            Logger.LogLine("Settings Loaded");
            SimGameState simulation = __instance.BattleTechGame.Simulation;
            SimGameConstants constants = simulation.Constants;
            float roll = simulation.NetworkRandom.Float(0f, 1f);
            Logger.LogLine("Rolled: " + roll);
            bool recovered = roll <= settings.RecoveryChance;
            if (!recovered) {
                Logger.LogLine("Mech not recovered");
                for (int i = 0; i < lostUnits.Count; i++)
                {
                    MechDef mech = lostUnits[i].mech;
                    if (mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                    {
                        Logger.LogLine("CT destroyed");
                        lostUnits[i].mechLost = true;
                        SalvageDef def = CompanyMechSalvage.CreateMechPart(__instance, constants, mech);
                        __instance.SalvageResults.Add(def);

                        foreach (MechComponentRef mechComponentRef in mech.Inventory)
                        {
                            if (!mech.IsLocationDestroyed(mechComponentRef.MountedLocation) && mechComponentRef.DamageLevel != ComponentDamageLevel.Destroyed)
                            {
                                __instance.SalvageResults.Add(new SalvageDef
                                {
                                    MechComponentDef = mechComponentRef.Def,
                                    Description = new DescriptionDef(mechComponentRef.Def.Description),
                                    RewardID = __instance.GenerateRewardUID(),
                                    Type = SalvageDef.SalvageType.COMPONENT,
                                    ComponentType = mechComponentRef.Def.ComponentType,
                                    Damaged = false,
                                    Count = 1
                                });
                            }
                        }
                    }
                    else if ((mech.IsLocationDestroyed(ChassisLocations.LeftLeg) && mech.IsLocationDestroyed(ChassisLocations.RightLeg)) || mech.IsLocationDestroyed(ChassisLocations.Head))
                    {
                        Logger.LogLine("Legs or Head destroyed");
                        lostUnits[i].mechLost = true;
                        SalvageDef def = CompanyMechSalvage.CreateMechPart(__instance, constants, mech);
                        __instance.SalvageResults.Add(def);
                        __instance.SalvageResults.Add(def);
                        foreach (MechComponentRef mechComponentRef in mech.Inventory)
                        {
                            if (!mech.IsLocationDestroyed(mechComponentRef.MountedLocation) && mechComponentRef.DamageLevel != ComponentDamageLevel.Destroyed)
                            {
                                __instance.SalvageResults.Add(new SalvageDef
                                {
                                    MechComponentDef = mechComponentRef.Def,
                                    Description = new DescriptionDef(mechComponentRef.Def.Description),
                                    RewardID = __instance.GenerateRewardUID(),
                                    Type = SalvageDef.SalvageType.COMPONENT,
                                    ComponentType = mechComponentRef.Def.ComponentType,
                                    Damaged = false,
                                    Count = 1
                                });
                            }
                        }
                    }
                    Logger.LogLine("Salvage created");
                }
            }
        } 
    }

    

    public static class CompanyMechSalvage
    {

        public static SalvageDef CreateMechPart(Contract contract, SimGameConstants sc, MechDef m)
        {
            SalvageDef salvageDef = new SalvageDef();
            salvageDef.Type = SalvageDef.SalvageType.MECH_PART;
            salvageDef.ComponentType = ComponentType.MechPart;
            salvageDef.Count = 1;
            salvageDef.Weight = sc.Salvage.DefaultMechPartWeight;
            DescriptionDef description = m.Description;
            DescriptionDef description2 = new DescriptionDef(description.Id, string.Format("{0} {1}", description.Name, sc.Story.DefaultMechPartName), description.Details, description.Icon, description.Cost, description.Rarity, description.Purchasable, description.Manufacturer, description.Model, description.UIName);
            salvageDef.Description = description2;
            salvageDef.RewardID = contract.GenerateRewardUID();
            return salvageDef;
        }

        public static void Init()
        {
            var harmony = HarmonyInstance.Create("de.morphyum.CompanyMechSalvage");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }

        public static Settings LoadSettings()
        {
            try
            {
                using (StreamReader r = new StreamReader("mods/CompanyMechSalvage/settings.json"))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<Settings>(json);
                }
            } catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
    }

    public class Settings
    {
        public float RecoveryChance;
    }

    public class Logger
    {
        public static void LogError(Exception ex)
        {
            string filePath = "mods/CompanyMechSalvage/Log.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        public static void LogLine(String line)
        {
            string filePath = "mods/CompanyMechSalvage/Log.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(line + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}