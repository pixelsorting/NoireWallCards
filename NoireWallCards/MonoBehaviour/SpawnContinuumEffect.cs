using Photon.Pun;
using UnityEngine;

namespace NoireWallCards
{

    // all this bullshit because rpc doesn't work without a proper photonview
    public class ContinuumAssets
    {
        private static GameObject _ContinuumWarp = null;

        internal static GameObject ContinuumWarp
        {
            get
            {
                if (_ContinuumWarp != null) { return ContinuumAssets._ContinuumWarp; }
                else
                {
                    _ContinuumWarp = new GameObject("ContinuumWarp", typeof(ContinuumEffect), typeof(PhotonView));
                    UnityEngine.GameObject.DontDestroyOnLoad(_ContinuumWarp);
                    return _ContinuumWarp;
                }
            }

            set { }
        }


    }

    public class SpawnContinuumEffect : MonoBehaviour
    {
        private static bool isInit = false;

        void Awake()
        {
            if (!isInit)
            {
                PhotonNetwork.PrefabPool.RegisterPrefab(ContinuumAssets.ContinuumWarp.name, ContinuumAssets.ContinuumWarp);
                FixedOutOfBoundsHelpers.SkipScreenEdgeBouncePatch = true;
            }
        }
        void Start()
        {
            if (!isInit)
            {
                isInit = true;
            }

            if (PhotonNetwork.OfflineMode || this.gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine)
            {
                PhotonNetwork.Instantiate(
                    ContinuumAssets.ContinuumWarp.name,
                    transform.position,
                    transform.rotation,
                    0,
                    new object[] { this.gameObject.transform.parent.GetComponent<PhotonView>().ViewID }
                );
            }
        }
    }
}