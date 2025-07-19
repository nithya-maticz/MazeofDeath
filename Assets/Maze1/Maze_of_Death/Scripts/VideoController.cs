using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class VideoController : MonoBehaviour
{
    public static VideoController Instance;

    [Header("Video Components")]
    [SerializeField] private VideoPlayer _videoPlayer;
    public RawImage VideoImage;
    public TMP_Text SubtitleText;
    public List<VideoData> Videos;

    private int currentIndex = 0;
    private bool isPlaying = false;
    private bool skipRequested = false;
    private Coroutine currentRoutine;
    private Coroutine subtitleRoutine;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // PlayNextVideo(); // optional
    }

    public void PlayNextVideo()
    {
        if (isPlaying || currentIndex >= Videos.Count)
        {
            if (currentIndex >= Videos.Count)
            {
                StartCoroutine(FinalCompleteRoutine());
            }
            return;
        }

        
        currentRoutine = StartCoroutine(PlayVideoRoutine(Videos[currentIndex]));
    }

    private IEnumerator FinalCompleteRoutine()
    {
        Game_Manager.Instance.Fade();
        //yield return new WaitForSeconds(0.15f);

        Game_Manager.Instance.StoryPage.SetActive(false);
        Game_Manager.Instance.LoadingPage.SetActive(false);
        Debug.Log("✨ All videos finished.");
        yield return null;
    }

    public void SkipAll()
    {
        if (!isPlaying && currentIndex >= Videos.Count)
            return;

        if (_videoPlayer.isPlaying)
            _videoPlayer.Stop();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (subtitleRoutine != null)
            StopCoroutine(subtitleRoutine);

        SubtitleText.text = "";
        VideoImage.gameObject.SetActive(false);

        foreach (var video in Videos)
        {
            if (video.TypeText != null)
                video.TypeText.text = "";
            if (video.BackGround != null)
                video.BackGround.gameObject.SetActive(false);
            video.IsPlayed = true;
        }

        currentIndex = Videos.Count;
        isPlaying = false;
        skipRequested = false;

        StartCoroutine(FinalCompleteRoutine());
    }

    public void Skip()
    {
        if (!isPlaying) return;

        skipRequested = true;

        if (_videoPlayer.isPlaying)
            _videoPlayer.Stop();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (subtitleRoutine != null)
            StopCoroutine(subtitleRoutine);

        SubtitleText.text = "";
        if (Videos[currentIndex].TypeText != null)
            Videos[currentIndex].TypeText.text = "";

        Videos[currentIndex].IsPlayed = true;
        currentIndex++;
        isPlaying = false;
        skipRequested = false;

        PlayNextVideo();
    }

    private IEnumerator PlayVideoRoutine(VideoData data)
    {
       
        Game_Manager.Instance.Fade();

        if (!Game_Manager.Instance.StoryPage.activeSelf)
            Game_Manager.Instance.StoryPage.SetActive(true);

        if (Game_Manager.Instance.LoadingPage.activeSelf)
            Game_Manager.Instance.LoadingPage.SetActive(false);

        isPlaying = true;
        SubtitleText.text = "";
        if (data.TypeText != null) data.TypeText.text = "";

        // 🔸 Prepare content
        VideoImage.gameObject.SetActive(false);
        if (data.BackGround != null)
            data.BackGround.gameObject.SetActive(false);
        if (data.TypeText != null)
            data.TypeText.gameObject.SetActive(false);

        if (!data.OnlyType)
        {
            if (!string.IsNullOrEmpty(data.Url))
            {
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, data.Url);
                _videoPlayer.url = path;
                _videoPlayer.Prepare();

                while (!_videoPlayer.isPrepared)
                {
                    if (skipRequested) yield break;
                    yield return null;
                }

                VideoImage.texture = _videoPlayer.texture;
            }
        }

        // 🔸 Activate content
        if (data.OnlyType)
        {
            data.BackGround.gameObject.SetActive(true);
            if (data.TypeText != null)
                data.TypeText.gameObject.SetActive(true);
        }
        else if (!string.IsNullOrEmpty(data.Url))
        {
            VideoImage.gameObject.SetActive(true);
            _videoPlayer.Play();
        }
        else if (data.BackGround != null)
        {
            data.BackGround.gameObject.SetActive(true);
        }

        // 🔸 Subtitle typing
        if (data.OnlyType && data.TypeText != null)
        {
            subtitleRoutine = StartCoroutine(TypeSubtitle(data.SubTitle, data.TypeText));
            yield return subtitleRoutine;
        }
        else
        {
            subtitleRoutine = StartCoroutine(TypeSubtitle(data.SubTitle, SubtitleText));
            yield return subtitleRoutine;
        }

        if (skipRequested) yield break;
        yield return new WaitForSeconds(3f);

        if (!data.OnlyType && _videoPlayer.isPlaying)
            _videoPlayer.Stop();

        data.IsPlayed = true;
        currentIndex++;
        isPlaying = false;
        skipRequested = false;

        SubtitleText.text = "";
        if (data.TypeText != null) data.TypeText.text = "";

        
        PlayNextVideo();
    }

    private IEnumerator TypeSubtitle(string subtitle, TMP_Text textComponent)
    {
        textComponent.text = "";
        foreach (char letter in subtitle)
        {
            if (skipRequested) yield break;
            textComponent.text += letter;
            yield return new WaitForSeconds(0.075f);
        }
    }
}

[Serializable]
public class VideoData
{
    public string Url;
    public bool IsPlayed;
    public bool OnlyType;
    public Image BackGround;
    public string SubTitle;
    public TMP_Text TypeText;
}
