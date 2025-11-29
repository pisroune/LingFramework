using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype
{
    public class SceneLoader
    {
        static SceneLoader()
        {
            ActionKit.OnUpdate.Register(Tick);
        }

        //º”‘ÿ≥°æ∞
        public static EasyEvent<float> OnLoading = new EasyEvent<float>();
        private static Action prgCB = null;
        public static void AsyncLoadScene(string sceneName, Action loaded = null)
        {
            AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
            prgCB = () =>
            {
                float val = sceneAsync.progress;
                OnLoading?.Trigger(val);
                if (val == 1)
                {
                    loaded?.Invoke();
                    prgCB = null;
                    sceneAsync = null;
                }
            };
        }

        static void Tick()
        {
            prgCB?.Invoke();
        }
    }
}