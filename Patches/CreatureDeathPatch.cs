using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace CureBlade.Patches
{
    [HarmonyPatch(typeof(CreatureDeath))]
    internal class CreatureDeathPatch
    {
        [HarmonyPatch(nameof(CreatureDeath.OnKillAsync))]
        [HarmonyPrefix]
        internal static void OnKillAsyncPrefix(CreatureDeath __instance)
        {
            CustomCreatureData creatureData = __instance.gameObject.GetComponent<CustomCreatureData>();

            GameObject gameObject = __instance.gameObject;

            TechType curedData = Utilities.curedCreatureList.GetOrDefault(CraftData.GetTechType(__instance.gameObject), TechType.None);

            Plugin.logger.LogInfo($"Last Damage Type: {creatureData.lastDamageType}");
            if(curedData != TechType.None && creatureData.lastDamageType == Plugin.dehydrationDamageType)
            {
                UWE.CoroutineHost.StartCoroutine(SpawnCuredFish(gameObject, curedData));
            }
        }

        [HarmonyPatch(nameof(CreatureDeath.OnTakeDamage))]
        [HarmonyPrefix]
        internal static void OnTakeDamagePrefix( CreatureDeath __instance, DamageInfo damageInfo)
        {
            __instance.gameObject.GetComponent<CustomCreatureData>().lastDamageType = damageInfo.type;
        }

        public static IEnumerator SpawnCuredFish( GameObject origFish, TechType curedFish)
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(curedFish, result, false);
            var gameObject = result.Get();
            gameObject.transform.position = origFish.transform.position;
            gameObject.transform.rotation = origFish.transform.rotation;
            gameObject.GetComponent<Rigidbody>().mass = origFish.GetComponent<Rigidbody>().mass;
            gameObject.GetComponent<Rigidbody>().velocity = origFish.GetComponent<Rigidbody>().velocity;
            gameObject.GetComponent<Rigidbody>().angularDrag = origFish.GetComponent<Rigidbody>().angularDrag * 3f;
            UnityEngine.Object.Destroy(origFish);
        }
    }
}
