using System;
using UnityEngine;

namespace FE.EasyAudio
{
    public static class AudioManager
    {
        public static event Action OnMuteChange;

        private const string volume_key = "VolumeKEY";
        private const string mute_key = "MuteKEY";

        private static float s_volumeLevel;

        static AudioManager()
        {
            s_volumeLevel = PlayerPrefs.GetFloat(volume_key, 1f);
            IsMuted = PlayerPrefs.GetInt(mute_key, 0) == 1;
        }

        public static float VolumeLevel
        {
            get { return IsMuted ? 0f : s_volumeLevel; }
        }

        public static bool IsMuted { get; private set; }

        public static void SetVolume(float v)
        {
            float value = Mathf.Clamp(v, 0f, 1f);
            if (value == 0f)
            {
                Mute();
            }
            else
            {
                s_volumeLevel = value;
                PlayerPrefs.SetFloat(volume_key, value);
                UnMute();
            }
        }

        public static void Mute()
        {
            IsMuted = true;
            PlayerPrefs.SetInt(mute_key, 1);
            OnMuteChange?.Invoke();
        }

        public static void UnMute()
        {
            IsMuted = false;
            PlayerPrefs.SetInt(mute_key, 0);
            OnMuteChange?.Invoke();
        }
    }
}