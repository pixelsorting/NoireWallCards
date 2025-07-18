using ModdingUtils.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;


// based on PCE's pac-bullets card


/*public class ContinuumEffect : MonoBehaviour
{
    PhotonView view;
    Transform parent;
    ProjectileHit projectile;
    OutOfBoundsHandler outOfBoundsHandler;

    void Awake()
    {
        this.parent = this.gameObject.transform.parent;
        if (this.parent == null) return;
        this.projectile = this.parent.GetComponent<ProjectileHit>();
        this.view = this.gameObject.GetComponent<PhotonView>();
        this.outOfBoundsHandler = CharacterDataExtension.GetAdditionalData(this.projectile.ownPlayer.data).outOfBoundsHandler;
    }

    void Update()
    {
        if (this.parent == null) return;

        Vector2 boundPos_ = OutOfBoundsHandlerExtensions.BoundsPointFromWorldPosition(outOfBoundsHandler, this.transform.position);
        Vector3 boundPos = new Vector3(boundPos_.x, boundPos_.y, this.transform.position.z);

        bool shouldWrap = false;

        if(boundPos.x >= 1f || boundPos.x <= 0f)
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
        // without disabling rendering the projectile can interact with the entire map while moving to the other side
        this.projectile.GetAdditionalData().inactiveDelay = float.MaxValue;

        TrailRenderer[] trailRenderers = this.projectile.GetComponentsInChildren<TrailRenderer>();
        Renderer[] renderers = this.projectile.GetComponentsInChildren<Renderer>();
        ParticleSystem[] particleSystems = this.projectile.GetComponentsInChildren<ParticleSystem>();

        foreach (Renderer r in renderers) { r.enabled = false; }
        foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
        foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }

        Vector2 wrapPos = OutOfBoundsHandlerExtensions.WorldPositionFromBoundsPoint(this.outOfBoundsHandler, new Vector2(x, y));
        this.parent.transform.position = new Vector3(wrapPos.x, wrapPos.y, z);

        this.ExecuteAfterFrames(3, () =>
        {
            foreach (Renderer r in renderers) { r.enabled = true; }
            foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
            foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }

            this.projectile.GetAdditionalData().inactiveDelay = 0f;
        });


    }
}*/

public class ContinuumEffect : MonoBehaviour
{
    //PhotonView view;
    Transform parent;
    ProjectileHit projectile;
    Player player;
    //OutOfBoundsHandler outOfBoundsHandler;
    RayHitReflect rayHitReflect;
    int bounceCount;

    void Start()
    {
        
        this.parent = this.gameObject.transform.parent;
        if (this.parent == null) return;
        
        rayHitReflect = parent.GetComponent<RayHitReflect>();
        bounceCount = 0;

        GetComponentInParent<SyncProjectile>().active = true;

        this.projectile = this.parent.GetComponent<ProjectileHit>();
        this.player = this.projectile.ownPlayer;
        //this.outOfBoundsHandler = CharacterDataExtension.GetAdditionalData(this.projectile.ownPlayer.data).outOfBoundsHandler;
    }

    void Update()
    {
        if (this.parent == null) return;

        Vector2 boundPos_ = OutOfBoundsHandlerExtensions.BoundsPointFromWorldPosition(CharacterDataExtension.GetAdditionalData(this.player.data).outOfBoundsHandler, this.transform.position);
        Vector3 boundPos = new Vector3(boundPos_.x, boundPos_.y, this.transform.position.z);

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
            DoWrapBullet(boundPos.x, boundPos.y, boundPos.z);
        }
    }

    private void DoWrapBullet(float x, float y, float z)
    {
        // without disabling rendering the projectile can interact with the entire map while moving to the other side
        this.projectile.GetAdditionalData().startTime = Time.time;
        this.projectile.GetAdditionalData().inactiveDelay = float.MaxValue;
        if (rayHitReflect != null)
        {
            bounceCount = rayHitReflect.reflects;
            rayHitReflect.reflects = 0;
        }

        TrailRenderer[] trailRenderers = this.projectile.GetComponentsInChildren<TrailRenderer>();
        Renderer[] renderers = this.projectile.GetComponentsInChildren<Renderer>();
        ParticleSystem[] particleSystems = this.projectile.GetComponentsInChildren<ParticleSystem>();

        foreach (Renderer r in renderers) { r.enabled = false; }
        foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
        foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }

        Vector2 wrapPos = OutOfBoundsHandlerExtensions.WorldPositionFromBoundsPoint(CharacterDataExtension.GetAdditionalData(this.projectile.ownPlayer.data).outOfBoundsHandler, new Vector2(x, y));
        this.parent.transform.position = new Vector3(wrapPos.x, wrapPos.y, z);

        this.ExecuteAfterFrames(4, () =>
        {
            foreach (Renderer r in renderers) { r.enabled = true; }
            foreach (TrailRenderer tr in trailRenderers) { tr.Clear(); }
            foreach (ParticleSystem ps in particleSystems) { ps.Clear(withChildren: true); }
            
            if(rayHitReflect != null) rayHitReflect.reflects = bounceCount;
            
            this.projectile.GetAdditionalData().inactiveDelay = 0f;
        });


    }
}