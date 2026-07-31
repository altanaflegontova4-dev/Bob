using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    [Header("Cutscenes")]
    public VideoPlayer cutscenePlayer;
    public GameObject cutscenePanel;

    [Header("Videos")]
    public VideoClip introCutscene;
    public VideoClip outroCutscene;

    [Header("Settings")]
    public float fadeSpeed = 2f;
    public CanvasGroup fadePanel;

    [Header("Scene Indices")]
    public int introSceneIndex = 0; // сцена где играет интро
    public int outroSceneIndex = 2; // сцена где играет аутро

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        // играем интро только на первой сцене
        if (currentScene == introSceneIndex && introCutscene != null)
        {
            StartCoroutine(PlayIntro());
        }
    }

    public void PlayOutro()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        // играем аутро только на третьей сцене
        if (currentScene == outroSceneIndex && outroCutscene != null)
        {
            StartCoroutine(PlayOutroSequence());
        }
    }

    IEnumerator PlayIntro()
    {
        if (PlayerController.instance != null)
            PlayerController.instance.enabled = false;

        yield return StartCoroutine(PlayCutscene(introCutscene));

        if (PlayerController.instance != null)
            PlayerController.instance.enabled = true;
    }

    IEnumerator PlayOutroSequence()
    {
        if (PlayerController.instance != null)
            PlayerController.instance.enabled = false;

        yield return StartCoroutine(PlayCutscene(outroCutscene));

        // после аутро загружаем главное меню
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator PlayCutscene(VideoClip clip)
    {
        if (clip == null || cutscenePlayer == null) yield break;

        if (cutscenePanel != null)
            cutscenePanel.SetActive(true);

        // fade in
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            while (fadePanel.alpha > 0f)
            {
                fadePanel.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        cutscenePlayer.clip = clip;
        cutscenePlayer.Prepare();

        while (!cutscenePlayer.isPrepared)
            yield return null;

        cutscenePlayer.Play();

        yield return new WaitForSeconds(0.5f);

        while (cutscenePlayer.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                cutscenePlayer.Stop();
                break;
            }
            yield return null;
        }

        if (cutscenePanel != null)
            cutscenePanel.SetActive(false);

        if (fadePanel != null)
            fadePanel.alpha = 0f;
    }
}