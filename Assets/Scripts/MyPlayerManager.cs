// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace Photon.Pun.Racer
{
#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class MyPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameObject LocalPlayerInstance;
        private bool leavingRoom;

        private int m_Racer;
        private int SkillInterval = 3;
        private float lastSkillTime = 0;

        private CarUserControl m_CarControl;
        
        [SerializeField] private SpriteRenderer[] racerRenders;

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            m_CarControl = GetComponentInChildren<CarUserControl>();

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                m_CarControl.SetIsMain(true);
            }
            
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            MyCarCamera _cameraWork = GameObject.FindObjectOfType<MyCarCamera>();
            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing(gameObject);
                    
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            //动物头像
            InitRacerRender();
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void InitRacerRender()
        {
            foreach (var render in racerRenders)
            {
                render.gameObject.SetActive(false);
            }

            m_Racer = photonView.Controller.GetPlayerRacer();
            racerRenders[m_Racer].gameObject.SetActive(true);

            if (m_Racer == 2)
            {
                SetLayerReceive(transform, LayerMask.NameToLayer("Fox"));
            }
        }

        void SetLayerReceive(Transform parent, int layer)
        {
            parent.gameObject.layer = layer;
            foreach (Transform child in parent)
            {
                SetLayerReceive(child.transform, layer);
            }
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();
            }
        }

        public override void OnLeftRoom()
        {
            this.leavingRoom = false;
        }

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            // if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            // {
            //     transform.position = new Vector3(0f, 5f, 0f);
            // }
        }

        #endregion

        #region Private Methods

#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        void ProcessInputs()
        {
            if (Input.GetKeyUp(KeyCode.B))
            {
                if (Time.time - lastSkillTime > SkillInterval)
                {
                    lastSkillTime = Time.time;
                    UserSkill();
                }
            }
        }

        void UserSkill()
        {
            switch (m_Racer)
            {
                case 0:
                    GetComponentInChildren<CarUserControl>().Jump();
                    break;
                case 1:
                    GetComponentInChildren<CarUserControl>().Accelarate();
                    break;
                case 2:
                    break;
                case 3:
                    GetComponentInChildren<CarUserControl>().GenObstacle();
                    break;
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
            }
            else
            {
                // Network player, receive data
            }
        }

        #endregion
    }
}