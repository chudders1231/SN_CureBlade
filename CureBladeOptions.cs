using Nautilus.Options;
using UnityEngine;

namespace CureBlade
{
    public class CureBladeOptions : ModOptions
    {
        public CureBladeOptions() : base("Cure Blade Options")
        {

            AddItem(Plugin.cureKnifeRange.ToModSliderOption(minValue: 0.1f, maxValue: 2.0f, step: 0.01f, floatFormat: "{0:F2}x"));
            AddItem(Plugin.cureKnifeDamage.ToModSliderOption(minValue: 0.1f, maxValue: 5.0f, step: 0.1f, floatFormat: "{0:F1}x"));

            ModSliderOption emissionStrength = Plugin.cureKnifeEmissionStrength.ToModSliderOption(minValue: 0.1f, maxValue: 2.0f, step: 0.01f, floatFormat: "{0:F2}x");
            emissionStrength.OnChanged += emissionStrengthChanged;
            AddItem(emissionStrength);
        }

        public void emissionStrengthChanged( object sender, SliderChangedEventArgs e)
        {
            Object.FindObjectsOfType<CureBladeComp>().ForEach(x => x.Refresh());
        }
    }
}