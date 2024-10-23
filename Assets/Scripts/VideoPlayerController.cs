using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class VideoPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class Video
    {
        public int id;
        public string video_link;
    }

    [System.Serializable]
    public class VideoList
    {
        public List<Video> videos;
    }

    public UnityEngine.Video.VideoPlayer videoPlayer;
    private List<string> videoLinks = new List<string>();

    void Awake()
    {
        StartCoroutine(GetActiveVideos());
    }

    IEnumerator GetActiveVideos()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:5000/videos/active");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            VideoList videoList = JsonUtility.FromJson<VideoList>("{\"videos\":" + request.downloadHandler.text + "}");
            foreach (Video video in videoList.videos)
            {
                videoLinks.Add(video.video_link);
            }

            StartCoroutine(PlayVideos());
        }
        else
        {
            Debug.LogError("Erro ao obter vídeos: " + request.error);
        }
    }

    IEnumerator PlayVideos()
    {
        while (true)
        {
            foreach (string link in videoLinks)
            {
                videoPlayer.url = link;
                videoPlayer.Play();
                yield return new WaitForSeconds((float)videoPlayer.length);
            }
        }
    }
}
