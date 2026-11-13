using BepInEx;
using HarmonyLib;

namespace ElinPartyMeals;

[BepInPlugin(Id, "Elin Party Meals", "1.0.0")]
internal sealed class Plugin : BaseUnityPlugin
{
    private const string Id = "raimumaker.elin.partymeals";

    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Id);
        Logger.LogInfo("Elin Party Meals loaded.");
    }
}
