using HarmonyLib;

namespace ElinPartyCare;

[HarmonyPatch(typeof(Chara), nameof(Chara.Tick))]
internal static class AutoMeal
{
    private static void Postfix(Chara __instance)
    {
        Chara ally = __instance;
        if (ally.IsPC || !ally.IsPCParty || !ally.IsAliveInCurrentZone ||
            ally.IsDeadOrSleeping || ally.hp <= 0 ||
            ally.hunger.GetPhase() < StatsHunger.Hungry)
        {
            return;
        }

        Chara? player = EClass.pc;
        if (player == null || !player.IsAliveInCurrentZone)
        {
            return;
        }

        Thing? food = player.things.Find(item =>
            item.Num > 0 && item.IsFood && !item.IsDecayed && ally.CanEat(item, shouldEat: true),
            recursive: true);

        if (food != null)
        {
            // Feed the ally a detached copy so the player's stack stays intact.
            ally.InstantEat(food.Duplicate(1), sound: false);
        }
    }
}
