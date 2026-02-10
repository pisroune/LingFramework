using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class PooledAudioClip : MonoBehaviour
    {
        float _timer;    //循环计时器
        float _localVol;    //初始音量

        public AudioSource ASource { get; private set; }
        public int Key { get; private set; }
        public bool Looping { get; private set; }  //循环设定
        public bool IsActive { get; private set; }

        public void Init(int key, Action<PooledAudioClip> stopAction)
        {
            Key = key;
            StopAction += stopAction;

            IsActive = false;
            ASource = GetComponent<AudioSource>();
        }

        public void Play(float localVol, bool loop)
        {
            IsActive = true;
            _localVol = localVol;
            Looping = loop;
            _timer = 0;

            ASource.Play();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= ASource.clip.length)
            {  //播放完毕自动停止
                StopAudio();
            }
        }

        public void UpdateVolume()
        {
            ASource.volume = _localVol * AudioSvc.AudioSetting.ClipVolume[Key];
        }

        /// <summary>
        /// 立刻停止音效
        /// </summary>
        public void StopAudio()
        {
            ASource?.Stop();
            StopAction?.Invoke(this);
            gameObject.SetActive(false);
        }
        public Action<PooledAudioClip> StopAction;
    }
}