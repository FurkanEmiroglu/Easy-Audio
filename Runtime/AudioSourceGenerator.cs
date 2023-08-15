using System.Collections.Generic;
using UnityEngine;

namespace FE.EasyAudio
{
    public class AudioSourceGenerator : MonoBehaviour
    {
        public static AudioSourceGenerator Instance
        {
            get
            {
                if (s_instance == null) return s_instance = new GameObject("AudioSourceGenerator").AddComponent<AudioSourceGenerator>();
                return s_instance;
            }
        }

        private static AudioSourceGenerator s_instance;

        private const int initial_step = 50;
        private const int expand_step = 10;
        
        private Queue<AudioSource> m_audioSources;
        private Transform m_parent;

        private void Awake()
        {
            s_instance = this;
            m_audioSources = new Queue<AudioSource>(initial_step);
            SpawnSource(initial_step);
            m_parent = transform;
        }

        public AudioSource Get()
        {
            if (m_audioSources.Count == 0) Expand();
            return m_audioSources.Dequeue();
        }

        public void Return(AudioSource source) => m_audioSources.Enqueue(source);

        private void Expand() => SpawnSource(expand_step);

        private void SpawnSource(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                AudioSource source = new GameObject("AudioSource").AddComponent<AudioSource>();
                source.transform.SetParent(Instance.transform);
                m_audioSources.Enqueue(source);
            }
        }
    }
}