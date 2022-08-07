using System;
using UnityEngine;

namespace Modelling.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SourceAudioPlayer : AudioPlayer
    {
        [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource ??= GetComponent<AudioSource>();
        }

        public override void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
}