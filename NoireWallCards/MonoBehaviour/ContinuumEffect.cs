using Photon.Pun;
using UnboundLib;
using UnityEngine;
using ModdingUtils.Extensions;

namespace NoireWallCards
{

    // based on PCE's pac-bullets card (except it should work now)
    [RequireComponent(typeof(PhotonView))]
    public class ContinuumEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        PhotonView view;
        Transform parent;
        ProjectileHit projectile;
        Player player;
        OutOfBoundsHandler outOfBoundsHandler;
        RayHitReflect rayHitReflect;
        int bounceCount;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;

            this.gameObject.transform.SetParent(parent.transform);
        }

        void Start()
        {

            this.parent = this.gameObject.transform.parent;
            if (this.parent == null) return;

            this.rayHitReflect = parent.GetComponent<RayHitReflect>();
            this.bounceCount = 0;

            // what the base game does but I guess direct RPC syncs better
            // GetComponentInParent<SyncProjectile>().active = true;

            this.projectile = this.parent.GetComponent<ProjectileHit>();
            this.player = this.projectile.ownPlayer;

            this.view = this.gameObject.GetComponent<PhotonView>();

            this.outOfBoundsHandler = this.player.GetComponent<ChildRPC>().childRPCs["OutOfBounds"].Target as OutOfBoundsHandler;
        }

        void Update()
        {
            if (this.parent == null) return;

            Vector2 boundPos_ = FixedOutOfBoundsHelpers.BoundsPointFromWorldPosition(this.outOfBoundsHandler, this.parent.transform.position);
            Vector3 boundPos = new Vector3(boundPos_.x, boundPos_.y, this.parent.transform.position.z);

            bool shouldWrap = false;

            if (boundPos.x >= 1f || boundPos.x <= 0f)
            {
                shouldWrap = true;
                boundPos.x = 1f - boundPos.x;
            }

            if (boundPos.y >= 1f || boundPos.y <= 0f)
            {
                shouldWrap = true;
                boundPos.y = 1f - boundPos.y;
            }
            if (shouldWrap)
            {
                if (PhotonNetwork.OfflineMode)
                {
                    this.RPCA_DoWrapBullet(boundPos.x, boundPos.y, boundPos.z);
                }
                else if (this.view.IsMine)
                {
                    this.view.RPC("RPCA_DoWrapBullet", RpcTarget.All, new object[] { boundPos.x, boundPos.y, boundPos.z });
                }
            }
        }

        [PunRPC]
        private void RPCA_DoWrapBullet(float x, float y, float z)
        {

            this.projectile.GetAdditionalData().startTime = Time.time;
            this.projectile.GetAdditionalData().inactiveDelay = float.MaxValue;

            // it took me 3 days to figure out how to fix out of bounds collisions and it was this one single line
            this.parent.GetComponent<RayCastTrail>().enabled = false;

            //FixedOutOfBoundsHelpers.SkipEmbigBouncePatch = false;

            // don't waste reflects bouncing off the border infinitely
            // ! Should not be needed anymore with the harmony patch !
            /* if (rayHitReflect != null)
            {
                
                this.bounceCount = rayHitReflect.reflects;
                this.rayHitReflect.reflects = 0;
            } */

            // without disabling rendering the projectile can interact with the entire map while moving to the other side
            TrailRenderer[] trailRenderers = this.projectile.GetComponentsInChildren<TrailRenderer>();
            Renderer[] renderers = this.projectile.GetComponentsInChildren<Renderer>();
            ParticleSystem[] particleSystems = this.projectile.GetComponentsInChildren<ParticleSystem>();

            foreach (Renderer r in renderers) { r.enabled = false; }
            foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
            foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }

            Vector2 wrapPos = this.outOfBoundsHandler.WorldPositionFromBoundsPoint(new Vector2(x, y));
            this.parent.transform.position = new Vector3(wrapPos.x, wrapPos.y, z);
            
            this.ExecuteAfterFrames(3, () =>
            {
                foreach (Renderer r in renderers) { r.enabled = true; }
                foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
                foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }
                
            });
            
            this.ExecuteAfterFrames(2, () =>
            {
                this.parent.GetComponent<RayCastTrail>().enabled = true;

                // ! Should not be needed with the harmony patch !
                //if (this.rayHitReflect != null) this.rayHitReflect.reflects = this.bounceCount; 

                this.projectile.GetAdditionalData().inactiveDelay = 0f;
            });

        }


    }
}