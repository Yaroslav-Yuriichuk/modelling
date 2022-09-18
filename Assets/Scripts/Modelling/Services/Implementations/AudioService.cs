using System;
using System.Collections.Generic;
using Modelling.Audio;
using UnityEngine;

namespace Modelling.Services
{
    public sealed class AudioService : IAudioService
    {
        private readonly AudioPlayer _audioPlayer;

        private Dictionary<SoundType, AudioClip> _clips = new Dictionary<SoundType, AudioClip>();

        public AudioService(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;

            foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
            {
                string path = $"SFX/SFX-{type}";
                AudioClip clip = Resources.Load<AudioClip>(path);
                
                _clips.Add(type, clip);
            }
        }
        
        public void PlaySound(SoundType type)
        {
            _audioPlayer.Play(_clips[type]);
        }
    }
}