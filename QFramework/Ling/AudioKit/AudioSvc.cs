using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

namespace QFramework
{
    public static class AudioSvc
    {
        public static IAudioSetting AudioSetting;
        public static Transform AudioTrans;       //音频物体的父物体

        //音频播放器  TODO 或许需要为每一个播放器写脚本
        public static Dictionary<int, AudioSource> AudioSourceDict;

        public static Dictionary<int, ClipListItem> PooledListDict;

        static List<AudioRely> _relyAudioList;
        static List<AudioRely> _removedRely;

        public static void Pause()
        {
            foreach (var item in AudioSourceDict)
            {
                if (!item.Value.ignoreListenerPause)
                {
                    item.Value.Pause();
                }
            }

            foreach (var item in PooledListDict)
            {
                foreach (var pooledClip in item.Value.ActivedSource)
                {
                    pooledClip.ASource.Pause();
                }
            }
        }
        public static void Resume()
        {
            foreach (var item in AudioSourceDict)
            {
                item.Value.UnPause();
            }
            foreach (var item in PooledListDict)
            {
                foreach (var pooledClip in item.Value.ActivedSource)
                {
                    pooledClip.ASource.UnPause();
                }
            }
        }

        public static void Init()
        {
            _relyAudioList = new List<AudioRely>();
            _removedRely = new List<AudioRely>();
            AudioSourceDict = new Dictionary<int, AudioSource>();
            PooledListDict = new Dictionary<int, ClipListItem>();

            AudioTrans = new GameObject().transform;
            GameObject.DontDestroyOnLoad(AudioTrans.gameObject);
            AudioTrans.name = "AudioKit";
        }

        public static void Update()
        {
            foreach (var item in _relyAudioList)
            {
                item.Update();
            }

            foreach (var item in _removedRely)
            {
                _relyAudioList.Remove(item);
            }

            foreach (var item in AudioSourceDict)
            {
                item.Value.volume = AudioSetting.SourceVolume[item.Key];
            }

            _removedRely.Clear();
        }

        /// <summary>
        /// 特点：单例、2D、全局
        /// </summary>
        /// <param name="index"></param>
        /// <param name="clip"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="ignorePause"></param>
        /// <returns></returns>
        public static AudioSource PlayAudioSource(int index, AudioClip clip, bool loop, float volume = 1, bool ignorePause = false)
        {
            MakeSureAudioSource(index);
            AudioSourceDict[index].clip = clip;
            AudioSourceDict[index].loop = loop;
            AudioSourceDict[index].volume = volume;
            AudioSourceDict[index].Play();
            AudioSourceDict[index].ignoreListenerPause = ignorePause;
            return AudioSourceDict[index];
        }
        public static void StopAudioSource(int index)
        {
            if (!AudioSourceDict.ContainsKey(index))
            {
                GameObject newGo = new GameObject();
                newGo.Parent(AudioTrans.gameObject.transform);
                AudioSource aSource = newGo.AddComponent<AudioSource>();
                AudioSourceDict.Add(index, aSource);
            }
            AudioSourceDict[index].Stop();
        }
        static void MakeSureAudioSource(int index)
        {
            if (!AudioSourceDict.ContainsKey(index))
            {
                GameObject newGo = new GameObject();
                newGo.Parent(AudioTrans.gameObject.transform);
                AudioSource aSource = newGo.AddComponent<AudioSource>();
                AudioSourceDict.Add(index, aSource);
            }
        }
        public static void MakeSureLowPassFilter(int index, float value)
        {
            if (!AudioSourceDict.ContainsKey(index))
            {
                MakeSureAudioSource(index);
            }
            var filter = AudioSourceDict[index].GetOrAddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = value;
        }
        public static void MakeSureHighPassFilter(int index, float value)
        {
            if (!AudioSourceDict.ContainsKey(index))
            {
                MakeSureAudioSource(index);
            }
            var filter = AudioSourceDict[index].GetOrAddComponent<AudioHighPassFilter>();
            filter.cutoffFrequency = value;
        }


        public static void InitClipListItem(int index, int size, int blend, bool loop)
        {
            if (!PooledListDict.ContainsKey(index))
            {
                PooledListDict.Add(index, new ClipListItem(index, size, blend, loop));
            }
            else
            {
                throw new Exception("重复构造，Key:" + index);
            }
        }
        public static void InitVoiceListItem(int index, int size, int blend, bool loop)
        {
            if (!PooledListDict.ContainsKey(index))
            {
                ClipListItem listItem = new ClipListItem(index, size, blend, loop);
                foreach (var pooledAudio in listItem.pooledAudio)
                {
                    pooledAudio.ASource.gameObject.AddComponent<AudioLowPassFilter>();
                    pooledAudio.ASource.gameObject.AddComponent<AudioHighPassFilter>();
                    pooledAudio.ASource.gameObject.AddComponent<AudioDistortionFilter>();
                }
                PooledListDict.Add(index, listItem);
            }
            else
            {
                throw new Exception("重复构造，Key:" + index);
            }
        }


        public static ClipListItem GetAudioClip(int index)
        {
            if (!PooledListDict.ContainsKey(index))
            {
                throw new Exception("没有构造这个Key:" + index);
            }
            return PooledListDict[index];
        }


        public static PooledAudioClip PlayClipAt(int index, AudioClip audioClip, Vector3 startPosition, float vol = 1,
            float pitch = 1.0f, int rolloffMode = 0, float minDist = 10f, float maxDist = 100.0f)
        {
            if (!PooledListDict.ContainsKey(index))
            {
                throw new Exception("没有构造这个Key:" + index);
            }
            return PooledListDict[index].PlayClipAt(audioClip, startPosition, vol, pitch, rolloffMode, minDist, maxDist);
        }


        public static PooledAudioClip PlayVoiceClipAt(int index, AudioClip audioClip, Vector3 startPosition, float vol = 1,
         float pitch = 1.0f, float blend = 0, int rolloffMode = 0, float minDist = 10f, float maxDist = 100.0f,
         float lowPassFilter = 5500, float highPassFilter = 50, float distortionFilter = 0)
        {
            if (!PooledListDict.ContainsKey(index))
            {
                throw new Exception("没有构造这个Key:" + index);
            }
            return PooledListDict[index].PlayVoiceClip(audioClip, startPosition, vol, pitch, blend, rolloffMode, minDist, maxDist, lowPassFilter, highPassFilter, distortionFilter);
        }

        /// <summary>
        /// 使音频依赖Transform
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="relyTrans"></param>
        /// <param name="inactiveTime"></param>
        public static void AddRely(PooledAudioClip clip, Transform relyTrans)
        {
            if (!clip.Looping)
            {
                throw new Exception("只能对循环音效使用");
            }
            if (clip.Looping)
            {
                _relyAudioList.Add(new AudioRely(clip, relyTrans, InactiveRely));
            }
        }
         static void InactiveRely(AudioRely rely)
        {
            _removedRely.Add(rely);
        }
    }
}
