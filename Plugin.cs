using Nautilus.Handlers;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx;
using CureBlade.Items.Equipment;

namespace CureBlade;

[BepInPlugin(myGUID, pluginName, versionString)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    // Plugin Setup
    private const string myGUID = "com.chadlymasterson.cureknife";
    private const string pluginName = "Cure Blade";
    private const string versionString = "1.0.0";
    public static readonly Harmony harmony = new Harmony(myGUID);
    public static ManualLogSource logger;

    // Dehydration Type Damage Enum
    public static DamageType dehydrationDamageType = EnumHandler.AddEntry<DamageType>("Dehydration");

    private void Awake()
    {
        // Patch in harmony changes
        harmony.PatchAll();
        
        // Run additional functions prior to registering items
        SetupConfigOptions.SetupBepinexConfigs();

        // Initialise custom prefabs
        InitializePrefabs();

        // Register project logging
        logger = Logger;

        logger.LogInfo($"Plugin {myGUID} is loaded!");
    }

    private void InitializePrefabs()
    {
        CureBladeItem.Patch();
    }
}