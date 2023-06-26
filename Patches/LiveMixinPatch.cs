using HarmonyLib;

namespace CureBlade.Patches
{
    [HarmonyPatch(typeof(LiveMixin))]
    internal class LiveMixinPatch
    {

        [HarmonyPatch(nameof(LiveMixin.Awake))]
        [HarmonyPrefix]
        internal static bool Awake_Prefix(LiveMixin __instance)
        {
            LiveMixin liveMixin = __instance;
            var go = liveMixin.gameObject;
            var crd = go.GetComponent<CreatureDeath>();

            if (liveMixin && go.GetComponent<EntityTag>() != null && crd != null)
            {
                var customCreatureData = liveMixin.gameObject.EnsureComponent<CustomCreatureData>();
            };
            return true;
        }
    }
}
