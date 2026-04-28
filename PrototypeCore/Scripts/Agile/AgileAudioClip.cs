using DistantLands.Lumen.Data;
using Project_TankSchool;
using QFramework;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu(fileName = "New AgileAudio", menuName = "Prototype/Agile AudioClip")]
    public class AgileAudioClip : ScriptableObject
    {
        public AudioClip[] clip;
        public bool is3D = true;
        [Range(0, 2)] public float volume = 1f;
        public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

        public void Play(Vector3 position = default)
        {
            if (clip == null) return;

            if (is3D)
            {
                AudioKit.PlayClipAt(clip.GetRandom(), position, volume);
            }
            else
            {
                AudioKit.Play2DClip(clip.GetRandom(), volume);
            }
        }

        [Button("ÒôÆµ×¢²á")]
        public void Register()
        {
            AgileAssets.RegisterAgileAudioClips();
        }
    }
}