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
    public Image ScreenFader;
    public TMP_Text SubtitleText;
    public List<VideoData> Videos;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private int currentIndex = 0;
    private bool isPlaying = false;
    private bool skipRequested = false;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PlayNextVideo();
    }

    public void PlayNextVideo()
    {
        if (isPlaying || currentIndex >= Videos.Count)
        {
            if (currentIndex >= Videos.Count)
            {
                Debug.Log("✅ All videos and subtitles are completed.");
            }
            return;
        }

        currentRoutine = StartCoroutine(PlayVideoRoutine(Videos[currentIndex]));
    }


    public void Skip()
    {
        if (!isPlaying) return;

        skipRequested = true;

        if (_videoPlayer.isPlaying)
            _videoPlayer.Stop();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        SubtitleText.text = "";
        Videos[currentIndex].IsPlayed = true;
        currentIndex++;
        isPlaying = false;
        skipRequested = false;
        PlayNextVideo();
    }

    private IEnumerator PlayVideoRoutine(VideoData data)
    {
        isPlaying = true;
        SubtitleText.text = "";
        if (data.TypeText != null) data.TypeText.text = "";

        // 🔸 Step 1: Fade out
        yield return StartCoroutine(FadeScreen(false));

        // 🔸 Step 2: Prepare content
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

        // 🔸 Step 3: Activate content
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

        // 🔸 Step 4: Fade in
        yield return StartCoroutine(FadeScreen(true));

        // 🔸 Step 5: Auto-type subtitle
        if (data.OnlyType && data.TypeText != null)
        {
            yield return StartCoroutine(TypeSubtitle(data.SubTitle, data.TypeText));
        }
        else
        {
            yield return StartCoroutine(TypeSubtitle(data.SubTitle, SubtitleText));
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

    private IEnumerator FadeScreen(bool fadeIn)
    {
        float elapsed = 0f;
        Color color = ScreenFader.color;
        float startAlpha = fadeIn ? 1f : 0f;
        float endAlpha = fadeIn ? 0f : 1f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            t = t * t * (3f - 2f * t); // smooth easing
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            ScreenFader.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        ScreenFader.color = color;
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
    public TMP_Text TypeText; // only used when OnlyType = true
}
