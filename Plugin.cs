using Nautilus.Handlers;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx;
using CureBlade.Items.Equipment;
using BepInEx.Configuration;

namespace CureBlade;

[BepInPlugin(myGUID, pluginName, versionString)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    // Config Stuff
    public static ConfigEntry<float> cureKnifeRange;
    public static ConfigEntry<float> cureKnifeDamage;

    // Plugin Setup
    private const string myGUID = "com.chadlymasterson.cureknife";
    private const string pluginName = "Cure Blade";
    private const string versionString = "1.0.1";
    public static readonly Harmony harmony = new Harmony(myGUID);
    public static ManualLogSource logger;

    // Dehydration Type Damage Enum
    public static DamageType dehydrationDamageType = EnumHandler.AddEntry<DamageType>("Dehydration");

    private void Awake()
    {
        // Patch in harmony changes
        harmony.PatchAll();
        
        // Run additional functions prior to registering items
        //SetupBepinexConfigs();

        // Initialise custom prefabs
        InitializePrefabs();

        // Register project logging
        logger = Logger;

        logger.LogInfo($"Plugin {myGUID} is loaded!");
    }

    private void SetupBepinexConfigs()
    {
        cureKnifeRange = Config.Bind("Cure Blade Options",
            "Cure Knife Range",
            1.0f,
            new ConfigDescription(
                "Changes the hit range of the cure knife.",
                new AcceptableValueRange<float>(0.1f, 2.0f)
            )
        );
        cureKnifeDamage = Config.Bind("Cure Blade Options",
            "Cure Knife Damage",
            1.0f,
            new ConfigDescription(
                "Changes the damage of the cure knife.",
                new AcceptableValueRange<float>(0.1f, 5.0f)
            )
        );

        OptionsPanelHandler.RegisterModOptions(new CureBladeOptions());
    }

    private void InitializePrefabs()
    {
        CureBladeItem.Patch();
    }
}