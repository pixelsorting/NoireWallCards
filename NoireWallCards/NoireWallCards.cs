using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnboundLib.GameModes;
using System;
using UnboundLib;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Photon.Pun;
using System.Runtime.CompilerServices;
using ModdingUtils.Extensions;
using System.Reflection;

namespace NoireWallCards
{
    // Hard deps
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.cardtheme.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    // Soft deps
    [BepInDependency("io.olavim.rounds.clientsideblock", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class NoireWallCards : BaseUnityPlugin
    {
        private const string ModId = "pixelsorting.rounds.noirewallcards";
        private const string ModName = "Noire Wall Cards";
        public const string Version = "0.1.9";
        internal static string modInitials = "NWC";

        internal static AssetBundle assets;
        void Awake()
        {
            assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("noire_assets", typeof(NoireWallCards).Assembly);
            assets.LoadAsset<GameObject>("_ModCards").GetComponent<CardHolder>().RegisterCards();
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            GameModeManager.AddHook(GameModeHooks.HookGameStart, ResetBouncePatchStatusOnGameStart);
        }

        private IEnumerator ResetBouncePatchStatusOnGameStart(IGameModeHandler _)
        {
            FixedOutOfBoundsHelpers.SkipScreenEdgeBouncePatch = false;
            yield return null;
        }
    }

    [HarmonyPatch(typeof(MapManager), "OnLevelFinishedLoading")]
    class MapManagerFinishedLoadingPatch
    {
        static void Postfix(MapManager __instance) 
        {
            FixedOutOfBoundsHelpers.Border = UIHandler.instance.transform.Find("Canvas/Border")?.transform;
            FixedOutOfBoundsHelpers.Border = FixedOutOfBoundsHelpers.Border ?? GameObject.Find("/BorderCanvas")?.transform;
        }
    }


    // might not be necessary anymore
    [HarmonyPatch(typeof(ScreenEdgeBounce), "Update")]
    public class MapEmbiggenerBouncePatch
    {
        [HarmonyBefore("pykess.rounds.plugins.mapembiggener")]
        [HarmonyPrefix]
        public static bool Prefix(ScreenEdgeBounce __instance, PhotonView ___view, ref bool ___done, Camera ___mainCam, ref Vector2 ___lastNormal, RayHitReflect ___reflect, ref float ___sinceBounce)
        {
            if(FixedOutOfBoundsHelpers.SkipScreenEdgeBouncePatch && ___view.IsMine)
            {
                ___done = true;
                return false;
            }
            return true;
        }
    }
}