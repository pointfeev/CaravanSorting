using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;
using UnityEngine;

namespace CaravanSorting
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("pointfeev.caravansorting");

            harmony.Patch(
                original: AccessTools.Method(typeof(CaravanUIUtility), "AddPawnsSections"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "AddPawnsSections")
            );

            bool GiddyUpCaravanInstalled = (from mod in ModLister.AllInstalledMods
                                            where mod.Active && mod.PackageId.ToLower() == "roolo.giddyupcaravan"
                                            select mod).Any<ModMetaData>();
            if (GiddyUpCaravanInstalled)
            {
                harmony.Patch(
                    original: AccessTools.Method(AccessTools.TypeByName("GiddyUpCaravan.Harmony.TransferableOneWayWidget_DoRow"), "handleAnimal"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), "HandleAnimal")
                );
            }
        }

        public static bool AddPawnsSections(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
            CaravanSorting.AddPawnsSections(widget, transferables);
            return false;
        }

        public static bool HandleAnimal(float num, Rect buttonRect, Pawn animal, ref List<Pawn> pawns, TransferableOneWay trad)
        {
            if (CaravanSorting.PawnsInCurrentSections != null)
            {
                pawns = CaravanSorting.PawnsInCurrentSections; // Fixes Giddy-up! Caravan rider selection functionality
            }
            return true;
        }
    }
}
