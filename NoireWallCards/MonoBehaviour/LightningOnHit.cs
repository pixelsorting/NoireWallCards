using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModdingUtils;
using ModdingUtils.RoundsEffects;
using Photon.Pun;
using SimulationChamber;
using System;
using HarmonyLib;

namespace NoireWallCards
{
    public class LightningOnHit : HitSurfaceEffect
    {
        GameObject thunderstruckParticleEffect;
        Player player;
        Gun gun;
        Gun defaultGun;
        SimulatedGun simGun;
        //public List<AudioClip> thunderAudioClips;
        AudioClip thunderAudio;
        
        void Awake()
        {
            thunderstruckParticleEffect = NoireWallCards.assets.LoadAsset<GameObject>("E_Thunderstruck");
            thunderAudio = NoireWallCards.assets.LoadAsset<AudioClip>("thunder0");
            defaultGun = Resources.Load<GameObject>("Player").GetComponent<Holding>().holdable.GetComponent<Gun>();
            // PhotonNetwork.PrefabPool.RegisterPrefab(lightningParticlePrefab.name, lightningParticlePrefab);
            this.player = this.GetComponentInParent<Player>();
            this.gun = this.player.data.weaponHandler.gun;
            simGun = new GameObject("Lightning-Gun").AddComponent<SimulatedGun>();
        }
        public override void Hit(Vector2 position, Vector2 normal, Vector2 velocity)
        {
            
            simGun.CopyGunStatsExceptActions(defaultGun);
            simGun.CopyAttackAction(defaultGun);
            simGun.CopyShootProjectileAction(defaultGun);

            simGun.damage = this.gun.damage * 0.12f;
            simGun.projectileSpeed = 9f;
            simGun.gravity = 0f;
            simGun.spread = 0f;
            simGun.reflects = 0;
            simGun.projectileSize = Math.Min(this.gun.damage / 30, 6);
            simGun.ammo = 3;
            simGun.bursts = 1;
            gun.timeBetweenBullets = 0.05f;
            simGun.ignoreWalls = true;
            simGun.attackSpeed = 0.05f;
            simGun.projectileColor = new Color(1f, 1f, 1f, 0f);
            simGun.soundDisableRayHitBulletSound = true;
            
            Vector3 spawnPosition = new Vector3(position.x, (46 + position.y), 0);
            
            Transform thunderTransform = thunderstruckParticleEffect.transform;
            GameObject.Instantiate(thunderstruckParticleEffect, spawnPosition, Quaternion.Euler(90,0,0));


            this.ExecuteAfterSeconds(0.55f, () => { AudioController.Play(thunderAudio, thunderTransform, true, 5f); } );
            

            if (!(player.data.view.IsMine || PhotonNetwork.OfflineMode)) { return; }
            
            simGun.SimulatedAttack(this.player.playerID, position, new Vector3(0f, 1f, 0f), 1f, 1f);
        }
    }
}