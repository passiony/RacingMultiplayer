using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Racer
{
    public class RacerPanel : MonoBehaviour
    {
        public Image[] images;
        public Button leftBtn;
        public Button rightBtn;

        private int curIndex = 0;

        void Start()
        {
            leftBtn.onClick.AddListener(OnLeftBtnClick);
            rightBtn.onClick.AddListener(OnRightBtnClick);
        }

        private void HideAll()
        {
            foreach (var img in images)
            {
                img.gameObject.SetActive(false);
            }
        }

        private void ShowNext(int offset)
        {
            curIndex += offset;
            if (curIndex >= images.Length)
            {
                curIndex = 0;
            }
            else if (curIndex < 0)
            {
                curIndex = images.Length - 1;
            }

            images[curIndex].gameObject.SetActive(true);
            PhotonNetwork.LocalPlayer.SetPlayerRacer(curIndex);
        }

        private void OnLeftBtnClick()
        {
            HideAll();
            ShowNext(-1);
        }

        private void OnRightBtnClick()
        {
            HideAll();
            ShowNext(1);
        }
    }
}