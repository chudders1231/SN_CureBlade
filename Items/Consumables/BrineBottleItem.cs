using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Ingredient = CraftData.Ingredient;
using UnityEngine;
using System.Collections.Generic;

namespace CureBlade.Items.Consumables
{
    internal class BrineBottleItem
    {
        public static PrefabInfo Info { get; private set; }
        public static void Patch()
        {
            Info = Utilities.CreatePrefabInfo("BrineSolution", "Concentrated Brine", "Usually used in curing meat, however using this high a concentration could potentially poison the consumer!", Utilities.GetSprite("brine_bottle_sprite"), 1, 1);

            var prefab = new CustomPrefab(Info);

            var clonePrefab = new CloneTemplate(Info, TechType.FilteredWater);

            clonePrefab.ModifyPrefab += obj =>
            {
                var eatable = obj.GetComponent<Eatable>();

                eatable.waterValue = -50;

                var renderer = obj.GetComponentInChildren<MeshRenderer>(true);
                obj.GetComponentsInChildren<MeshRenderer>(true).ForEach(x => x.material.mainTexture = Utilities.GetTexture("brine_bottle"));
            };

            prefab.SetGameObject(clonePrefab);
            prefab.SetUnlock(TechType.HeatBlade).CompoundTechsForUnlock = new List<TechType>() { TechType.FiltrationMachine };

            prefab.Register();

        }
    }
}
