using System.Reflection;
using HarmonyLib;
using Verse;

namespace WarframeMAV
{
    [StaticConstructorOnStartup]
    public static class HarmonyStartUp
    {
        static HarmonyStartUp()
        {
            new Harmony("akreedz.rimworld.warframemav").PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("Warframe M-A-V Harmony Add");
        }
    }
}