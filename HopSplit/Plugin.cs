using BepInEx;
using HarmonyLib;
using HopSplit.UI;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HopSplit
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid      = "ninjacookie.hops.hopsplit";
        public const string pluginName      = "Hop Split";
        public const string pluginVersion   = "1.0.1";

        public void Awake()
        {
            var harmony = new Harmony(pluginGuid);
            harmony.PatchAll();

            DataHandler.LoadAll();

            LiveSplit.LiveSplitManager.Connect();
            UIHandler.CreateUIObject();
        }
    }
}