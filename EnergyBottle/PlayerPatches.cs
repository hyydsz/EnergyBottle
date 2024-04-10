using HarmonyLib;
using UnityEngine;

namespace EnergyBottle
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.Start))]
        public static void Start(Player __instance)
        {
            if (__instance.IsLocal)
            {
                __instance.gameObject.AddComponent<PlayerEnergyManager>();
            }
        }
    }

    public class PlayerEnergyManager : MonoBehaviour
    {
        public static PlayerEnergyManager instance;

        private float DefaultJump = 0;
        private float DefaultSpeed = 0;

        public float JumpBoostTime = 0;
        public float JumpBoostForce = 0;

        public float SpeedBoostTime = 0;
        public float SpeedBoostForce = 0;

        private PlayerController player;

        void Start()
        {
            instance = this;

            player = GetComponent<PlayerController>();
            DefaultJump = player.jumpImpulse;
            DefaultSpeed = player.movementForce;
        }
        void Update()
        {
            if (JumpBoostTime > 0)
            {
                JumpBoostTime -= Time.deltaTime;

                player.jumpImpulse = JumpBoostForce;
            }
            else
            {
                player.jumpImpulse = DefaultJump;
            }

            if (SpeedBoostTime > 0)
            {
                SpeedBoostTime -= Time.deltaTime;

                player.movementForce = SpeedBoostForce;
            }
            else
            {
                player.movementForce = DefaultSpeed;
            }
        }
    }
}
