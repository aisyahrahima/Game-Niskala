
using UnityEngine;

    public class EndingMelancholicBGM : MonoBehaviour
    {
        public static EndingMelancholicBGM Instance { get; private set; }

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            audioSource = GetComponentInChildren<AudioSource>(true);

            DontDestroyOnLoad(gameObject);
        }

        public void PlayBGM()
        {
            Debug.Log("PlayBGM DIPANGGIL!");

            if (audioSource == null)
            {
                Debug.LogError("AudioSource TIDAK DITEMUKAN!");
                return;
            }

            audioSource.Play();

            Debug.Log("Status BGM: " + audioSource.isPlaying);
        }

        public void StopBGM()
        {
            Debug.Log("STOP BGM DIPANGGIL!");

            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Destroy(gameObject);
        }
    }
