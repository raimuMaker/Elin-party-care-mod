using HarmonyLib;

namespace ElinPartyCare;

[HarmonyPatch(typeof(Chara), nameof(Chara.Tick))]
internal static class AutoBandage
{
    private const string BandageId = "bandage";

    private static void Postfix(Chara __instance)
    {
        Chara ally = __instance;
        if (ally.IsPC || !ally.IsPCParty || !ally.IsAliveInCurrentZone ||
            ally.IsDeadOrSleeping || ally.hp <= 0 || ally.hp * 2L > ally.MaxHP ||
            ally.HasCondition<ConBandage>())
        {
            return;
        }

        Chara? player = EClass.pc;
        if (player == null || !player.IsAliveInCurrentZone)
        {
            return;
        }

        foreach (Thing item in player.things)
        {
            if (item.id != BandageId || item.Num < 1)
            {
                continue;
            }

            // The normal use action consumes only this detached copy.
            Thing bandage = item.Duplicate(1);
            var useBandage = bandage.trait.GetHealAction(ally);
            if (useBandage != null)
            {
                useBandage();
                Plugin.LogDebug("Applied a bandage to a party member.");
            }

            return;
        }
    }
}
