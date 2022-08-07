using UnityEngine;

namespace Modelling.Audio
{
    public abstract class AudioPlayer : MonoBehaviour
    {
        public abstract void Play(AudioClip clip);
    }
}