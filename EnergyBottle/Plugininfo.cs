using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ShopUtils;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization;

namespace EnergyBottle
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("hyydsz-ShopUtils")]
    public class PluginInfo : BaseUnityPlugin
    {
        public const string ModGUID = "hyydsz-EnergyBottle";
        public const string ModName = "EnergyBottle";
        public const string ModVersion = "0.1.1";

        public static ConfigEntry<float> SpawnRarity;
        public static ConfigEntry<int> SpawnBudgetCost;

        private readonly Harmony harmony = new Harmony(ModGUID);

        private AssetBundle asset;

        void Awake()
        {
            LoadBottle();
            LoadLangauge();

            harmony.PatchAll();
        }

        private void LoadBottle()
        {
            asset = QuickLoadAssetBundle("energy");
            EnergyType.Config = Config;

            Item jump = asset.LoadAsset<Item>("JumpBottle");
            Item health = asset.LoadAsset<Item>("HealthBottle");
            Item oxygen = asset.LoadAsset<Item>("OxygenBottle");
            Item speed = asset.LoadAsset<Item>("SpeedBottle");

            SpawnRarity = Config.Bind("Config", "SpawnRarity", 1f);
            SpawnBudgetCost = Config.Bind("Config", "SpawnBudgetCost", 1);

            JumpBehavior.jump = new EnergyType(true, "JumpBottle", 30f, 30f, 50, jump, typeof(JumpBehavior), "Jump Height");
            HealthBehavior.health = new EnergyType(false, "HealthBottle", 30f, 0, 50, health, typeof(HealthBehavior), "Restore Health");
            OxygenBehavior.oxygen = new EnergyType(false, "OxygenBottle", 30f, 0, 50, oxygen, typeof(OxygenBehavior), "Restore Oxygen");
            SpeedBehavior.speed = new EnergyType(true, "SpeedBottle", 25f, 30f, 50, speed, typeof(SpeedBehavior), "Movement Speed");
        }

        private void LoadLangauge()
        {
            Locale Chinese = Languages.GetLanguage("zh-Hans");
            Languages.AddLanguage("JumpBottle_ToolTips", "[鼠标左键] 使用", Chinese);
            Languages.AddLanguage("JumpBottle", "跳跃能量瓶", Chinese);

            Languages.AddLanguage("HealthBottle_ToolTips", "[鼠标左键] 使用", Chinese);
            Languages.AddLanguage("HealthBottle", "血量能量瓶", Chinese);

            Languages.AddLanguage("OxygenBottle_ToolTips", "[鼠标左键] 使用", Chinese);
            Languages.AddLanguage("OxygenBottle", "氧气能量瓶", Chinese);

            Languages.AddLanguage("SpeedBottle_ToolTips", "[鼠标左键] 使用", Chinese);
            Languages.AddLanguage("SpeedBottle", "速度能量瓶", Chinese);
        }

        public static AssetBundle QuickLoadAssetBundle(string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name);
            return AssetBundle.LoadFromFile(path);
        }
    }
}
