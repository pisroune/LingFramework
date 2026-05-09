using Project_TankSchool;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu(fileName = "New AgileVfx", menuName = "Prototype/Agile AgileVfx")]
    public class AgileVfx : ScriptableObject
    {
        public ParticleVfxBase VfxBase;

        public ParticleVfxBase Play(VfxPlayArgs args)
        {
            return VfxBase.Play(args);
        }

#if UNITY_EDITOR
        [Button("ÌØÐ§×¢²á")]
        public void Register()
        {
            AgileAssets.RegisterVfx();
        }
#endif
    }
}
