using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoUI : MonoBehaviour
{
    public static VideoUI Instance;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        CameraManager_Lobby.Instance.ChangeCam(0, () =>
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            UIManager_Lobby.Instance.trm.gameObject.SetActive(true);
            GameManager_Lobby._instance._isVideo = false;
            GameManager_Lobby._instance._pC.IsMoveTrue = true;
        });
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)&&gameObject.activeSelf && GameManager_Lobby._instance._isVideo)
        {
            Hide();

        }
    }
}
