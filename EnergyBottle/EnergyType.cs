using BepInEx.Configuration;
using ShopUtils;
using ShopUtils.Network;
using System;
using UnityEngine;

namespace EnergyBottle
{
    public class EnergyType
    {
        public static ConfigFile Config;

        private ConfigEntry<bool> Buyable;
        private ConfigEntry<float> Time;
        private ConfigEntry<float> Force;
        private ConfigEntry<int> Price;

        private string EnergyName;
        private bool HasTime = true;

        private float DefualtForce;
        private float DefualtTime;
        private int DefaultPrice;

        private string Description;

        private Item item;

        private string TimeKey
        {
            get { return $"{EnergyName}Time"; }
        }

        private string BuyableKey
        {
            get { return $"{EnergyName}Buyable"; }
        }

        private string PriceKey
        {
            get { return $"{EnergyName}Price"; }
        }

        private string ForceKey
        {
            get { return $"{EnergyName}Force"; }
        }

        public float m_Time;
        public float m_Force;

        public bool SteamLoaded = false;

        public EnergyType(bool HasTime, string EnergyName, float Force, float Time, int Price, Item item, Type type, string Description) {

            this.EnergyName = EnergyName;
            this.HasTime = HasTime;
            this.item = item;

            this.Description = Description;

            DefualtForce = Force;
            DefualtTime = Time;
            DefaultPrice = Price;

            item.itemObject.AddComponent(type);

            InitData();

            Items.RegisterSpawnableItem(item, PluginInfo.SpawnRarity.Value, PluginInfo.SpawnBudgetCost.Value);
        }

        private void InitData()
        {
            Networks.OnLobbyCreated += () =>
            {
                Buyable = Config.Bind(EnergyName, "BottleBuyable", false);
                Price = Config.Bind(EnergyName, "BottlePrice", DefaultPrice);
                Force = Config.Bind(EnergyName, "BottleForce", DefualtForce, Description);

                if (HasTime)
                {
                    Time = Config.Bind(EnergyName, "BottleTime", DefualtTime);

                    Networks.SetLobbyData(TimeKey, Time.Value);
                }

                Networks.SetLobbyData(BuyableKey, Buyable.Value);
                Networks.SetLobbyData(PriceKey, Price.Value);
                Networks.SetLobbyData(ForceKey, Force.Value);
            };

            Networks.OnLobbyEnter += () =>
            {
                try
                {
                    if (HasTime)
                    {
                        m_Time = float.Parse(Networks.GetLobbyData(TimeKey));
                    }

                    m_Force = float.Parse(Networks.GetLobbyData(ForceKey));

                    bool buyable = bool.Parse(Networks.GetLobbyData(BuyableKey));
                    if (buyable)
                    {
                        int price = int.Parse(Networks.GetLobbyData(PriceKey));

                        Items.RegisterShopItem(item, ShopItemCategory.Misc, price);
                    }

                    SteamLoaded = true;
                }
                catch
                {
                    Debug.LogError("Load Energy Bottles Data Fail.");
                }
            };
        }
    }
}
