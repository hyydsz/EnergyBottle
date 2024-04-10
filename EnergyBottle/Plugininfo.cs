using BepInEx;
using HarmonyLib;
using Steamworks;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace EnergyBottle
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("hyydsz-ShopUtils")]
    public class PluginInfo : BaseUnityPlugin
    {
        public const string ModGUID = "hyydsz-EnergyBottle";
        public const string ModName = "EnergyBottle";
        public const string ModVersion = "0.1.0";

        private readonly Harmony harmony = new Harmony(ModGUID);

        private static List<EnergyType> energyTypes;

        private AssetBundle asset;

        private CSteamID steamID;

        void Awake()
        {
            LoadBottle();
            LoadSteamConfig();

            harmony.PatchAll();
        }

        private void LoadBottle()
        {
            asset = QuickLoadAssetBundle("energy");

            Item jump = asset.LoadAsset<Item>("JumpBottle");
            Item health = asset.LoadAsset<Item>("HealthBottle");
            Item oxygen = asset.LoadAsset<Item>("OxygenBottle");
            Item speed = asset.LoadAsset<Item>("SpeedBottle");

            JumpBehavior.jump = new EnergyType(true, "JumpBottle", 30f, 30f, 50, jump, typeof(JumpBehavior));
            HealthBehavior.health = new EnergyType(false, "HealthBottle", 30f, 0, 50, health, typeof(HealthBehavior));
            OxygenBehavior.oxygen = new EnergyType(false, "OxygenBottle", 30f, 0, 50, oxygen, typeof(OxygenBehavior));
            SpeedBehavior.speed = new EnergyType(true, "SpeedBottle", 25f, 30f, 50, speed, typeof(SpeedBehavior));

            energyTypes = new List<EnergyType>()
            {
                JumpBehavior.jump,
                HealthBehavior.health,
                OxygenBehavior.oxygen,
                SpeedBehavior.speed
            };
        }

        private void LoadSteamConfig()
        {
            Callback<LobbyEnter_t>.Create((steam) =>
            {
                steamID = new CSteamID(steam.m_ulSteamIDLobby);

                energyTypes.ForEach(item => item.SteamLoad(steamID));
            });

            Callback<LobbyCreated_t>.Create((steam) =>
            {
                if (steam.m_eResult == EResult.k_EResultOK)
                {
                    steamID = new CSteamID(steam.m_ulSteamIDLobby);

                    energyTypes.ForEach(item => item.SteamSet(steamID, Config));
                }
            });
        }

        public static void AddEnergyType(EnergyType type)
        {
            energyTypes.Add(type);
        }

        public static AssetBundle QuickLoadAssetBundle(string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name);
            return AssetBundle.LoadFromFile(path);
        }
    }
}
