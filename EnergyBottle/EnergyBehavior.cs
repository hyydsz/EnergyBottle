using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace EnergyBottle
{
    public abstract class EnergyBehavior : ItemInstanceBehaviour
    {
        private OnOffEntry onOffEntry;

        private bool on;

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

            on = onOffEntry.on;
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
                if (onOffEntry.on)
                {
                    if (CanUse())
                    {
                        onOffEntry.on = false;
                        onOffEntry.SetDirty();
                    }
                }
            }

            if (on != onOffEntry.on)
            {
                Use.Play();

                if (isHeldByMe)
                {
                    PlayerController player = Player.localPlayer.refs.controller;
                    player.StartCoroutine(OnUse(player));

                    if (Player.localPlayer.TryGetInventory(out var inventory))
                    {
                        inventory.TryRemoveItemFromSlot(Player.localPlayer.data.selectedItemSlot, out _);
                    }
                }
            }
        }

        public abstract bool CanUse();
        public abstract IEnumerator OnUse(PlayerController player);
    }

    public class JumpBehavior : EnergyBehavior
    {
        private static bool jumpCoolingTime = true;
        public static EnergyType jump;

        public override bool CanUse()
        {
            return jump.SteamLoaded && jumpCoolingTime;
        }

        public override IEnumerator OnUse(PlayerController player)
        {
            jumpCoolingTime = false;

            float defaultJump = player.jumpImpulse;
            player.jumpImpulse = jump.m_Force;

            yield return new WaitForSeconds(jump.m_Time);

            player.jumpImpulse = defaultJump;

            jumpCoolingTime = true;
        }
    }

    public class HealthBehavior : EnergyBehavior
    {
        public static EnergyType health;

        public override bool CanUse()
        {
            return Player.localPlayer.data.health < Player.PlayerData.maxHealth && health.SteamLoaded;
        }

        public override IEnumerator OnUse(PlayerController player)
        {
            Player.localPlayer.data.health = Mathf.MoveTowards(Player.localPlayer.data.health,
                Player.PlayerData.maxHealth, health.m_Force);

            yield break;
        }
    }

    public class OxygenBehavior : EnergyBehavior
    {
        public static EnergyType oxygen;

        public override bool CanUse()
        {
            return Player.localPlayer.data.remainingOxygen < Player.localPlayer.data.maxOxygen && oxygen.SteamLoaded;
        }

        public override IEnumerator OnUse(PlayerController player)
        {
            Player.localPlayer.data.remainingOxygen = Mathf.MoveTowards(Player.localPlayer.data.remainingOxygen,
                 Player.localPlayer.data.maxOxygen, oxygen.m_Force);

            yield break;
        }
    }

    public class SpeedBehavior : EnergyBehavior
    {
        private static bool speedCoolingTime = true;
        public static EnergyType speed;

        public override bool CanUse()
        {
            return speed.SteamLoaded && speedCoolingTime;
        }

        public override IEnumerator OnUse(PlayerController player)
        {
            speedCoolingTime = false;

            float defaultSpeed = player.movementForce;
            player.movementForce = speed.m_Force;

            yield return new WaitForSeconds(speed.m_Time);

            player.jumpImpulse = defaultSpeed;

            speedCoolingTime = true;
        }
    }
}
