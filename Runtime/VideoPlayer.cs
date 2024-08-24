using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS
{

    public class VideoPlayer : MonoBehaviour
    {

        public Camera TargetCamera { get; set; }
        UnityEngine.Video.VideoPlayer player;

        public bool isPlaying { get; private set; } = false;

        private void Awake()
        {
            TargetCamera = Camera.main;
            player = this.GetComponent<UnityEngine.Video.VideoPlayer>();
            // Play on awake defaults to true. Set it to false to avoid the url set
            // below to auto-start playback since we're in Start().
            player.targetCamera = TargetCamera;
            player.playOnAwake = false;

            // By default, VideoPlayers added to a camera will use the far plane.
            // Let's target the near plane instead.
            player.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

            // Each time we reach the end, we slow down the playback by a factor of 10.
            player.loopPointReached += EndReached;
            // This will cause our Scene to be visible through the video being played.
            player.targetCameraAlpha = 0.5F;
        }
        public void Dispose()
        {
            GameObject.Destroy(this.gameObject,1f);
        }
        public static VideoPlayer Create()
        {
            GameObject go = new GameObject("VideoPlayer");
            go.AddComponent<UnityEngine.Video.VideoPlayer>();
            return go.AddComponent<VideoPlayer>();
        }

        public void Play(string url, bool loop = false)
        {
            player.url = System.IO.Path.Combine(Application.streamingAssetsPath, url);
            player.isLooping = loop;
            player.Play();
            isPlaying = true;
        }

        public IEnumerator waitDone()
        {
            yield return null;
            while (isPlaying)
            {
                yield return null;
            }
        }

        public void Stop()
        {
            player.Stop();
            isPlaying = false;
        }

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            if (!player.isLooping)
            {
                this.Stop();
            }
            Debug.Log("Video Stoped");
        }
    }
}
