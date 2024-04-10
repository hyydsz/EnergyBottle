using BepInEx.Configuration;
using ShopUtils;
using Steamworks;
using System;
using UnityEngine;

namespace EnergyBottle
{
    public class EnergyType
    {
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

        public bool m_Buyable;
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

            Items.RegisterSpawnableItem(item, Item.RARITY.always, PluginInfo.SpawnBudgetCost.Value);
        }

        public void SteamSet(CSteamID steamID, ConfigFile config)
        {
            Buyable = config.Bind(EnergyName, "BottleBuyable", false);
            Price = config.Bind(EnergyName, "BottlePrice", DefaultPrice);
            Force = config.Bind(EnergyName, "BottleForce", DefualtForce, Description);
            
            if (HasTime)
            {
                Time = config.Bind(EnergyName, "BottleTime", DefualtTime);

                SteamMatchmaking.SetLobbyData(steamID, TimeKey, Time.Value.ToString());
            }

            SteamMatchmaking.SetLobbyData(steamID, BuyableKey, Buyable.Value.ToString());
            SteamMatchmaking.SetLobbyData(steamID, PriceKey, Price.Value.ToString());
            SteamMatchmaking.SetLobbyData(steamID, ForceKey, Force.Value.ToString());
        }

        public void SteamLoad(CSteamID steamID)
        {
            try
            {
                m_Buyable = bool.Parse(SteamMatchmaking.GetLobbyData(steamID, BuyableKey));
                m_Force = float.Parse(SteamMatchmaking.GetLobbyData(steamID, ForceKey));

                if (HasTime)
                {
                    m_Time = float.Parse(SteamMatchmaking.GetLobbyData(steamID, TimeKey));
                }

                if (m_Buyable)
                {
                    int price = int.Parse(SteamMatchmaking.GetLobbyData(steamID, PriceKey));

                    Items.RegisterShopItem(item, ShopItemCategory.Misc, price);
                }

                SteamLoaded = true;
            }
            catch
            {
                Debug.LogError("Energy Bottle Get Data Error!");
            }
        }
    }
}
