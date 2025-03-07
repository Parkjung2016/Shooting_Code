using System.Collections;

using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class LoadingSceneManager : MonoBehaviour

{

    public static string nextScene;




    private float progressBar;

    private TextMeshProUGUI _loadingText;

    private TextMeshProUGUI _helpText;

    [SerializeField]
    private string[] _helpString;

    private Image _fade;

    private Image _helpImg;
    [SerializeField]
    private Sprite[] _helpSprites;
    private void Awake()
    {
        Transform canvas = FindObjectOfType<Canvas>().transform;
        _helpImg = canvas.Find("Mask/Image").GetComponent<Image>();
        _fade = canvas.Find("Fade").GetComponent<Image>();
        _loadingText = canvas.Find("Loading").GetComponent<TextMeshProUGUI>();
        _helpText = canvas.Find("Help").GetComponent<TextMeshProUGUI>();
        _fade.DOFade(0, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
       LoadScene();
        progressBar = 0;
        StartCoroutine(LoadingText());
        StartCoroutine(HelpText());
    }
    IEnumerator HelpText()
    {
        while (true)
        {
            int ran = Random.Range(0, _helpString.Length);
            _helpText.text = "<bounce>" + _helpString[ran] + "</bounce>";
            _helpImg.sprite = _helpSprites[ran];
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }
    IEnumerator LoadingText()
    {
        TMP_TextInfo info = _loadingText.textInfo;
        int cnt = 6;
        int dotCount = 4;

        while (true)
        {
            dotCount--;
            _loadingText.maxVisibleCharacters = cnt - dotCount;
            yield return new WaitForSeconds(0.8f);
            if (dotCount == 0)
                dotCount = 4;
        }
    }


    public static void LoadScene(string sceneName)

    {

        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");

    }


    private void LoadScene()
    {
        DOTween.To(() => progressBar, x => progressBar = x, 1, 5).OnComplete(()=>
        {
            Camera.main.GetComponent<AudioSource>().DOFade(0, 1);
            _fade.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene(nextScene));
        });



    }
}
