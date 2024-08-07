﻿using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using CureBlade;
using CureBlade.Items.Consumables;
using System.Net;

namespace CureBlade.Items.Equipment
{
    public static class CureBladeItem
    {
        public static PrefabInfo Info { get; private set; } = Utilities.CreatePrefabInfo("BrineBlade", "Brine Blade", "Cures small organisms by converting body water content into brine.", Utilities.GetSprite("brine_blade_sprite"), 1, 1);

        public static void Patch()
        {

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
                obj.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => {
                    x.material.SetTexture("_Illum", Utilities.GetTexture("brine_blade_illum"));
                });

            };


            var recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Knife, 1),
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(BrineBottleItem.Info.TechType, 2)
                }
            };

            prefab.SetGameObject(clonePrefab);
            prefab.SetUnlock(TechType.HeatBlade);
            prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);
            prefab.AddGadget(new EquipmentGadget(prefab, EquipmentType.Hand)).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithStepsToFabricatorTab("Tools")
                .WithCraftingTime(5.5f);

            prefab.Register();
        }
    }
}

public class CureBladeComp : HeatBlade
{
    public override string animToolName { get; } = TechType.HeatBlade.AsString(true);
    public float emissionStrength;

    public override void OnToolUseAnim(GUIHand hand)
    {
        var heatBladeDamage = 40;

        this.damage = heatBladeDamage * Plugin.cureKnifeDamage.Value;
        base.OnToolUseAnim(hand);

        GameObject hitObj = null;
        Vector3 hitPosition = default;
        UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, attackDist * Plugin.cureKnifeRange.Value, ref hitObj, ref hitPosition);

    }
    public override void OnDraw(Player p)
    {
        base.OnDraw(p);

        Refresh();
    }

    public void Refresh()
    {
        emissionStrength = Plugin.cureKnifeEmissionStrength.Value;

        this.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => {
            if (x.material.GetColor("_GlowColor") == Color.white * emissionStrength) return;
                
            x.material.SetColor("_GlowColor", Color.white * emissionStrength);
        });
    }
}