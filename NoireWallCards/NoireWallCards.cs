using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;

namespace NoireWallCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.cardtheme.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class NoireWallCards : BaseUnityPlugin
    {
        private const string ModId = "pixelsorting.rounds.noirewallcards";
        private const string ModName = "Noire Wall Cards";
        public const string Version = "0.1.5";
        internal static string modInitials = "NWC";

        internal static AssetBundle assets;
        void Awake()
        {
            assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("noire_assets", typeof(NoireWallCards).Assembly);
            assets.LoadAsset<GameObject>("_ModCards").GetComponent<CardHolder>().RegisterCards();
        }

        void Start()
        {

        }
    }
}