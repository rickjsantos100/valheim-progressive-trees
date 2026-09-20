using BepInEx;
using Jotunn.Utils;

namespace ProgressiveTrees
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal sealed class ProgressiveTreesPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.progressivetrees.mod";
        public const string PluginName = "Progressive Trees";
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
        }
    }
}
