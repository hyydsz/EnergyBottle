using BepInEx.Configuration;
using ShopUtils;
using ShopUtils.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnergyBottle
{
    public class EnergyType
    {
        public static ConfigFile Config;

        private string EnergyName;
        private bool HasTime = true;

        private float DefualtForce;
        private float DefualtTime;
        private int DefaultPrice;

        private string Description;

        private Item item;

        private string TimeKey
        {
            get { return $"{EnergyName}Sconds"; }
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

            LoadConfigData();

            Items.RegisterSpawnableItem(item, PluginInfo.SpawnRarity.Value, PluginInfo.SpawnBudgetCost.Value);
        }

        private void LoadConfigData()
        {
            Dictionary<string, object> datas = new Dictionary<string, object>
            {
                {BuyableKey, Config.Bind(EnergyName, "BottleBuyable", false).Value},
                {PriceKey, Config.Bind(EnergyName, "BottlePrice", DefaultPrice).Value},
                {ForceKey,  Config.Bind(EnergyName, "BottleForce", DefualtForce, Description).Value},
            };

            if (HasTime) {
                datas.Add(TimeKey, Config.Bind(EnergyName, "BottleTime", DefualtTime).Value);
            }

            Networks.SetNetworkSync(datas,
                dic =>
                {
                    try
                    {
                        if (HasTime) {
                            m_Time = float.Parse(dic[TimeKey]);
                        }

                        m_Force = float.Parse(dic[ForceKey]);

                        bool buyable = bool.Parse(dic[BuyableKey]);
                        int price = int.Parse(dic[PriceKey]);

                        if (buyable) {
                            Items.RegisterShopItem(item, ShopItemCategory.Misc, price);
                        }

                        Debug.Log($"{EnergyName}: [Force: {m_Force}, Time: {m_Time}, Buyable: {buyable}, Price: {price}]");

                        SteamLoaded = true;
                    }
                    catch
                    {
                        Debug.LogError($"Load `{EnergyName}` Energy Bottles Data Fail.");
                    }
                }
            );
        }
    }
}
