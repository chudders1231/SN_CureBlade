using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using CureBlade;

namespace CureBlade.Items.Equipment
{
    public static class CureBladeItem
    {
        public static PrefabInfo Info;

        public static void Patch()
        {
            Info = Utilities.CreatePrefabInfo("CureBlade", "Cure Blade", "Cures small organisms by converting body water content into brine.", Utilities.GetSprite("brine_blade_sprite"), 1, 1);

            var prefab = new CustomPrefab(Info);

            var clonePrefab = new CloneTemplate(Info, TechType.HeatBlade);
            clonePrefab.ModifyPrefab += obj =>
            {
                var heatBlade = obj.GetComponent<HeatBlade>();
                var cureKnife = obj.AddComponent<CureBladeComp>().CopyComponent(heatBlade);

                Object.DestroyImmediate(heatBlade);

                cureKnife.damageType = Plugin.dehydrationDamageType;
                cureKnife.vfxEventType = VFXEventTypes.diamondBlade;

                var renderer = obj.GetComponentInChildren<MeshRenderer>(true);
                obj.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => x.material.mainTexture = Utilities.GetTexture("brine_blade"));
                obj.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => x.material.SetTexture("_Illum", Utilities.GetTexture("brine_blade_illum")));
            };


            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Knife, 1),
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(TechType.Salt, 4)
                }
            };

            prefab.SetGameObject(clonePrefab);
            prefab.SetUnlock(TechType.HeatBlade);
            prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);
            prefab.AddGadget(new EquipmentGadget(prefab, EquipmentType.Hand)).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Fabricator)
                .WithStepsToFabricatorTab("Personal", "Tools")
                .WithCraftingTime(5.5f);

            prefab.Register();
        }
    }
}

public class CureBladeComp : HeatBlade
{
    public override string animToolName { get; } = TechType.HeatBlade.AsString(true);

    public override void OnToolUseAnim(GUIHand hand)
    {
        var heatBladeDamage = 40;

        this.damage = heatBladeDamage * SetupConfigOptions.cureKnifeDamage.Value;
        base.OnToolUseAnim(hand);

        GameObject hitObj = null;
        Vector3 hitPosition = default;
        UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, attackDist * SetupConfigOptions.cureKnifeRange.Value, ref hitObj, ref hitPosition);

    }
}