using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatUI : MonoBehaviour
{
    private bool _isOpened;
    List<KeyCode> cheatCommands = new List<KeyCode>();
    KeyCode[] cheatCheck = new KeyCode[] {KeyCode.UpArrow, KeyCode.UpArrow , KeyCode.DownArrow, KeyCode.DownArrow,
                                         KeyCode.LeftArrow,KeyCode.RightArrow,KeyCode.LeftArrow
                                        ,KeyCode.RightArrow,KeyCode.B,KeyCode.A};
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(setCheatCommand(KeyCode.UpArrow));
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
                StartCoroutine(setCheatCommand(KeyCode.DownArrow));
            if (Input.GetKeyDown(KeyCode.RightArrow))
                StartCoroutine(setCheatCommand(KeyCode.RightArrow));
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                StartCoroutine(setCheatCommand(KeyCode.LeftArrow));
            if (Input.GetKeyDown(KeyCode.B))
                StartCoroutine(setCheatCommand(KeyCode.B));
            if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(setCheatCommand(KeyCode.A));

        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            cheatCommands.Clear();
        }
    }
    IEnumerator setCheatCommand(KeyCode keyCode)
    {

        cheatCommands.Add(keyCode);
        if (cheatCommands.Count >= 10)
        {
            byte check = 0;
            for (int i = 0; i < cheatCommands.Count; i++)
            {
                if (cheatCommands[i] == cheatCheck[i])
                    check++;
            }
            if (check == 10)
            {
                _isOpened = !_isOpened;
                transform.GetChild(0).gameObject.SetActive(_isOpened);
                cheatCommands.Clear();
            }

        }
        yield return new WaitForSeconds(3);
        cheatCommands.Remove(keyCode);
    }
    public void AddMoney()
    {
        PlayerDataManager.Instance.PlayerData.Gold += 10000;
        ShopUI.Instance.SetGoldText();
        ShopUI.Instance.CheckPet();
    }
}
