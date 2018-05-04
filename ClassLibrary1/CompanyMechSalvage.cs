using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using BattleTech;
using System.Reflection;

namespace CompanyMechSalvage
{
    [HarmonyPatch(typeof(Contract), "GenerateSalvage")]
    public static class Contract_GenerateSalvage_Patch
    {
        static void Postfix(Contract __instance, List<UnitResult> enemyMechs, List<VehicleDef> enemyVehicles, List<UnitResult> lostUnits, bool logResults = false)
        {
            SimGameState simulation = __instance.BattleTechGame.Simulation;
            SimGameConstants constants = simulation.Constants;
            for (int i = 0; i < lostUnits.Count; i++)
            {
                MechDef mech = lostUnits[i].mech;
                if (mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                {
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
    }
}