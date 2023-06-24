using Nautilus.Options;
using BepInEx;
using BepInEx.Configuration;
using System.IO;
using Nautilus.Handlers;

namespace CureBlade
{
    public class SetupConfigOptions
    {
        // Declare config options
        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "CureBlade.cfg"), true);

        public static ConfigEntry<float> cureKnifeRange;
        public static ConfigEntry<float> cureKnifeDamage;

        public static void SetupBepinexConfigs()
        {
            cureKnifeRange = config.Bind("Cure Blade Options",
                "Cure Knife Range",
                1.0f,
                new ConfigDescription(
                    "Changes the hit range of the cure knife.",
                    new AcceptableValueRange<float>(0.1f, 2.0f)
                )
            );
            cureKnifeDamage = config.Bind("Cure Blade Options",
                "Cure Knife Damage",
                1.0f,
                new ConfigDescription(
                    "Changes the damage of the cure knife.",
                    new AcceptableValueRange<float>(0.1f, 5.0f)
                )
            );

            OptionsPanelHandler.RegisterModOptions(new CureBladeOptions());
        }
    }

    public class CureBladeOptions : ModOptions
    {
        public CureBladeOptions() : base("Cure Blade Options")
        {
            AddItem(SetupConfigOptions.cureKnifeRange.ToModSliderOption(minValue: 0.1f, maxValue: 2.0f, step: 0.01f, floatFormat: "{0:F2}x"));
            AddItem(SetupConfigOptions.cureKnifeDamage.ToModSliderOption(minValue: 0.1f, maxValue: 5.0f, step: 0.1f, floatFormat: "{0:F1}x"));
        }
    }
}