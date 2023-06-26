using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.Racer;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        [SerializeField] private int jump = 10;
        [SerializeField] private int Accel = 10;
        [SerializeField] private Transform endPos;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private bool IsMine;

        public void SetIsMain(bool ismain)
        {
            IsMine = ismain;
        }
        private void FixedUpdate()
        {
            if (!IsMine)
            {
                return;
            }
            if (!MyGameManager.Instance.StartGame)
            {
                return;
            }

            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }

        public void Jump()
        {
            gameObject.GetComponent<Rigidbody>().velocity += new Vector3(0, jump, 0);
        }

        public void Accelarate()
        {
            gameObject.GetComponent<Rigidbody>().velocity += transform.forward * Accel;
        }

        public void GenObstacle()
        {
            PhotonNetwork.Instantiate("Obstacle", endPos.position, Quaternion.identity, 0);
        }
    }
}