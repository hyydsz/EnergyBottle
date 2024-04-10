using Photon.Pun;
using UnityEngine;

namespace EnergyBottle
{
    public abstract class EnergyBehavior : ItemInstanceBehaviour
    {
        private OnOffEntry onOffEntry;

        private SFX_PlayOneShot Use;
        private Material Middle;

        public override void ConfigItem(ItemInstanceData data, PhotonView playerView)
        {
            if (!data.TryGetEntry(out onOffEntry))
            {
                onOffEntry = new OnOffEntry()
                {
                    on = true
                };

                data.AddDataEntry(onOffEntry);
            }

            if (!onOffEntry.on)
            {
                Middle.SetColor("_Color", new Color(0, 0, 0));
            }
        }

        void Awake()
        {
            Use = transform.Find("SFX/Use").GetComponent<SFX_PlayOneShot>();
            Middle = transform.Find("Bottle/Middle").GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            if (isHeldByMe && !Player.localPlayer.HasLockedInput() && Player.localPlayer.input.clickWasPressed)
            {
                if (CanUse())
                {
                    if (onOffEntry.on)
                    {
                        Use.Play();

                        OnUse();

                        onOffEntry.on = false;
                        onOffEntry.SetDirty();
                    }
                }
                
            }
        }

        public abstract bool CanUse();
        public virtual void OnUse()
        {
            if (Player.localPlayer.TryGetInventory(out var inventory))
            {
                inventory.TryRemoveItemFromSlot(Player.localPlayer.data.selectedItemSlot, out _);
            }
        }
    }

    public class JumpBehavior : EnergyBehavior
    {
        public static EnergyType jump;

        public override bool CanUse()
        {
            return jump.SteamLoaded;
        }

        public override void OnUse()
        {
            base.OnUse();

            PlayerEnergyManager.instance.JumpBoostTime += jump.m_Time;
            PlayerEnergyManager.instance.JumpBoostForce = jump.m_Force;
        }
    }

    public class HealthBehavior : EnergyBehavior
    {
        public static EnergyType health;

        public override bool CanUse()
        {
            return Player.localPlayer.data.health < Player.PlayerData.maxHealth && health.SteamLoaded;
        }

        public override void OnUse()
        {
            base.OnUse();

            Player.localPlayer.data.health = Mathf.MoveTowards(Player.localPlayer.data.health,
                Player.PlayerData.maxHealth, health.m_Force);
        }
    }

    public class OxygenBehavior : EnergyBehavior
    {
        public static EnergyType oxygen;

        public override bool CanUse()
        {
            return Player.localPlayer.data.remainingOxygen < Player.localPlayer.data.maxOxygen && oxygen.SteamLoaded;
        }

        public override void OnUse()
        {
            base.OnUse();

            Player.localPlayer.data.remainingOxygen = Mathf.MoveTowards(Player.localPlayer.data.remainingOxygen,
                 Player.localPlayer.data.maxOxygen, oxygen.m_Force);
        }
    }

    public class SpeedBehavior : EnergyBehavior
    {
        public static EnergyType speed;

        public override bool CanUse()
        {
            return speed.SteamLoaded;
        }

        public override void OnUse()
        {
            base.OnUse();

            PlayerEnergyManager.instance.SpeedBoostTime += speed.m_Time;
            PlayerEnergyManager.instance.SpeedBoostForce = speed.m_Force;
        }
    }
}
