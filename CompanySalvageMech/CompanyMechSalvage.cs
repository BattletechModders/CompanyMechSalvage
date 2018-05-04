using System;
using Harmony;
using BattleTech;

namespace CompanyMechSalvage
{
    [HarmonyPatch(typeof(VersionInfo), "GetReleaseVersion")]
    public static class VersionInfo_GetReleaseVersion_Patch
    {
        static void Postfix(ref string __result)
        {
            string old = __result;
            __result = old + " w/ BTML";
        }
    }

    public static class VersionMod
    {
        public static void Init()
        {
            var harmony = HarmonyInstance.Create("de.morphyum.CompanyMechSalvage");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
