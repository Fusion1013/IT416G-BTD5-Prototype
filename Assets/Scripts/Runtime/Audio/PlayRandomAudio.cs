using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomAudio : MonoBehaviour
    {
        public List<AudioClip> clips;
        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void PlayAudio()
        {
            var clip = clips[Random.Range(0, clips.Count)];
            _source.PlayOneShot(clip);
        }
    }
}
