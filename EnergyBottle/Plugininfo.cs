using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ShopUtils;
using ShopUtils.Language;
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
        public const string ModVersion = "0.1.4";

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
            Locale Chinese = Languages.GetLanguage(LanguageEnum.ChineseSimplified);
            Chinese.AddLanguage(
                new LanguageInstance("JumpBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("JumpBottle", "跳跃能量瓶"),
                new LanguageInstance("HealthBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("HealthBottle", "血量能量瓶"),
                new LanguageInstance("OxygenBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("OxygenBottle", "氧气能量瓶"),
                new LanguageInstance("SpeedBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("SpeedBottle", "速度能量瓶")
                );

            Locale ChineseTraditional = Languages.GetLanguage(LanguageEnum.ChineseTraditional);
            ChineseTraditional.AddLanguage(
                new LanguageInstance("JumpBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("JumpBottle", "跳躍能量瓶"),
                new LanguageInstance("HealthBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("HealthBottle", "血量能量瓶"),
                new LanguageInstance("OxygenBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("OxygenBottle", "氧氣能量瓶"),
                new LanguageInstance("SpeedBottle_ToolTips", "[LMB] 使用"),
                new LanguageInstance("SpeedBottle", "速度能量瓶")
                );

            Locale English = Languages.GetLanguage(LanguageEnum.English);
            English.AddLanguage(
                new LanguageInstance("JumpBottle_ToolTips", "[LMB] Use"),
                new LanguageInstance("JumpBottle", "JumpBottle"),
                new LanguageInstance("HealthBottle_ToolTips", "[LMB] Use"),
                new LanguageInstance("HealthBottle", "HealthBottle"),
                new LanguageInstance("OxygenBottle_ToolTips", "[LMB] Use"),
                new LanguageInstance("OxygenBottle", "OxygenBottle"),
                new LanguageInstance("SpeedBottle_ToolTips", "[LMB] Use"),
                new LanguageInstance("SpeedBottle", "SpeedBottle")
                );
        }

        public static AssetBundle QuickLoadAssetBundle(string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name);
            return AssetBundle.LoadFromFile(path);
        }
    }
}
