using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private NPCInfo _npcInfo;



    [SerializeField]
    private float _dialogDis;
    private GameObject _canvas;

    public Coroutine updateCoroutine;

    [SerializeField]
    private int _index;
    private void Awake()
    {
        _canvas = transform.Find("NPCCanvas").gameObject;

    }
    private void Start()
    {

        updateCoroutine= StartCoroutine(update());
    }
    public void DialogStart()
    {
        DialogUI.Instance.StartDialog(_npcInfo._npcName, _npcInfo._npcDialogArrary[0]._npcDialog);
    }
    public void DialogEnd()
    {
        DialogUI.Instance.EndDialog(false);

    }
   public IEnumerator update()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager_Lobby._instance._pC != null);
            if (PauseUI.Instance.Paused) yield return null;
            _canvas.SetActive(Vector3.Distance(GameManager_Lobby._instance._pC.transform.position, transform.position) <= _dialogDis);
            if (Vector3.Distance(GameManager_Lobby._instance._pC.transform.position, transform.position) <= _dialogDis)
            {
                if (Input.GetKeyDown(KeyCode.F) && !DialogUI.Instance.Dialoging)
                {
                    DialogUI.Instance.CurrentNPC = _index;
                    DialogStart();
                }
            }
            else if (DialogUI.Instance.Dialoging && DialogUI.Instance.CurrentNPC == _index&& Vector3.Distance(GameManager_Lobby._instance._pC.transform.position, transform.position) > _dialogDis + 1f)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                DialogUI.Instance.CurrentNPC = 99;
                DialogEnd();

            }

            //yield return null;
        }
    }

}
