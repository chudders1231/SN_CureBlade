using CureBlade.Items.Consumables;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace CureBlade.Patches
{
    [HarmonyPatch(typeof(FiltrationMachine))]
    internal class FiltrationMachinePatch
    {
        [HarmonyPatch(typeof(FiltrationMachine), nameof(FiltrationMachine.TryFilterWater))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> TranspilerWater(IEnumerable<CodeInstruction> instructions)
        {
           return new CodeMatcher(instructions).MatchForward(true,
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldc_I4, (int)TechType.BigFilteredWater))
                .InsertAndAdvance( new CodeInstruction(OpCodes.Ldarg_0) )
                .SetInstruction( new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FiltrationMachinePatch), "GetBiomeForWater", parameters: new[] { typeof(FiltrationMachine) })) )
                .InstructionEnumeration();
        }

        static TechType GetBiomeForWater(FiltrationMachine __instance) {
            // Get Biome code goes here
            string biome = LargeWorld.main.GetBiome(__instance.transform.position);

            // Check if biome returns true -> return new techtype
            if(biome.Contains("LostRiver"))
            {
                UWE.CoroutineHost.StartCoroutine(SetInstanceWaterPrefab(__instance, TechType.FilteredWater));
                return TechType.FilteredWater;
            }
            // Else -> return old techtype

            UWE.CoroutineHost.StartCoroutine(SetInstanceWaterPrefab(__instance, TechType.BigFilteredWater));
            return TechType.BigFilteredWater;
        }
        public static IEnumerator SetInstanceWaterPrefab(FiltrationMachine __instance, TechType waterTechType)
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(waterTechType, false, result);
            var gameObject = result.Get();

            __instance.waterPrefab = gameObject;
        }

        [HarmonyPatch(typeof(FiltrationMachine), nameof(FiltrationMachine.TryFilterSalt))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> TranspilerSalt(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(true,
                 new CodeMatch(OpCodes.Callvirt),
                 new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)TechType.Salt))
                 .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                 .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FiltrationMachinePatch), "GetBiomeForSalt", parameters: new[] { typeof(FiltrationMachine) })))
                 .InstructionEnumeration();
        }

        static TechType GetBiomeForSalt(FiltrationMachine __instance)
        {
            // Get Biome code goes here
            string biome = LargeWorld.main.GetBiome(__instance.transform.position);

            // Check if biome returns true -> return new techtype
            if (biome.Contains("LostRiver"))
            {
                UWE.CoroutineHost.StartCoroutine(SetInstanceSaltPrefab(__instance, BrineBottleItem.Info.TechType));
                return TechType.FilteredWater;
            }
            // Else -> return old techtype

            UWE.CoroutineHost.StartCoroutine(SetInstanceSaltPrefab(__instance, TechType.Salt));
            return TechType.Salt;
        }
        public static IEnumerator SetInstanceSaltPrefab(FiltrationMachine __instance, TechType saltTechType)
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.GetPrefabForTechTypeAsync(saltTechType, false, result);
            var gameObject = result.Get();

            __instance.waterPrefab = gameObject;
        }
    }
}
