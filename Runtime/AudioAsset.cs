using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FE.EasyAudio
{
    [CreateAssetMenu(menuName = "Easy Audio/Audio Asset", order = 0)]
    public class AudioAsset : ScriptableObject
    {
        // clip

        [SerializeField] private AudioClip _clip;

        [SerializeField] private float _originalDuration;

        [SerializeField] private float _usedDuration;

        // volume settings

        [SerializeField] private Vector2 _normalizedTime = new(0, 1);

        [SerializeField] private bool _useCurvedVolume;

        [SerializeField] private float _fixedVolume = 1f;

        [SerializeField] private AnimationCurve _volumeCurve;

        [SerializeField] private PitchPreference _pitchPreference;

        [SerializeField] private float _fixedPitch = 1f;

        [SerializeField] private AnimationCurve _pitchCurve;

        [SerializeField] private float _pitchRandomDifference;

        public async void Play()
        {
            if (AudioManager.IsMuted) return;
            if (!Application.isPlaying) return;

            AudioSource source = AudioSourceGenerator.Instance.Get();

            SetSourceClip(source);
            SetVolume(source);
            SetPitch(source);

            source.Play();

            await Task.Delay((int)(_usedDuration * 1000));
            #if UNITY_EDITOR
if (Application.isPlaying) AudioSourceGenerator.Instance.Return(source);
            #endif
        }

        private void SetSourceClip(AudioSource source)
        {
            source.clip = _clip;
            source.time = _originalDuration * _normalizedTime.x;
        }

        private void SetPitch(AudioSource source)
        {
            switch (_pitchPreference)
            {
                case PitchPreference.Fixed:
                    source.pitch = _fixedPitch;
                    break;

                case PitchPreference.Curve:
                    source.pitch = _pitchCurve.Evaluate(0);

                    DOVirtual.Float(0, 1f, _usedDuration, SetValue).SetEase(Ease.Linear).SetLink(source.gameObject).Play();

                    break;


                case PitchPreference.Random:
                    source.pitch = _fixedPitch + Random.Range(-_pitchRandomDifference, _pitchRandomDifference);
                    break;
            }
            
            void SetValue(float x)
            {
                source.pitch = _pitchCurve.Evaluate(x);
            }
        }

        private void SetVolume(AudioSource source)
        {
            if (_useCurvedVolume)
            {
                source.volume = _volumeCurve.Evaluate(0);

                DOVirtual.Float(0, 1, _usedDuration, SetValue).SetEase(Ease.Linear).SetLink(source.gameObject).Play();
            }
            else
            {
                source.volume = _fixedVolume * AudioManager.VolumeLevel;
            }

            void SetValue(float x)
            {
                source.volume = x;
            }
        }
    }
}