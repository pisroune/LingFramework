using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace QFramework
{
    public class ClipListItem
    {
        private int _key;
        private Transform _parent;
        private int _blend;
        private bool _loopAudio;

        public List<PooledAudioClip> pooledAudio;

        private int nextActive = 0;
        public List<PooledAudioClip> ActivedSource { get; private set; } = new List<PooledAudioClip>();  //正在播放中的音效

        public ClipListItem(int key, int spawnSize, int blend, bool loop)
        {
            _key = key;
            _blend = blend;
            _loopAudio = loop;
            _parent = new GameObject("ClipItem_" + _key.ToString()).transform;
            _parent.SetParent(AudioSvc.AudioTrans);

            pooledAudio = new List<PooledAudioClip>();
            for (int i = 0; i < spawnSize; i++)
            {
                PooledAudioClip temp = NewItem();
                temp.gameObject.name = i.ToString();
                pooledAudio.Add(temp);
            }
        }

        public void UpdateVolume()
        {
            for (int i = 0; i < ActivedSource.Count; i++)
            {
                ActivedSource[i].UpdateVolume();
            }
        }

        PooledAudioClip NewItem()
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = _parent.position;
            gameObject.transform.rotation = _parent.rotation;
            gameObject.transform.SetParent(_parent);
            gameObject.gameObject.SetActive(false);

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.dopplerLevel = 0;
            source.spatialBlend = _blend;
            source.loop = _loopAudio;

            PooledAudioClip item = gameObject.AddComponent<PooledAudioClip>();
            item.Init(_key, DeActiveClip);
            return item;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="clip">音频片段</param>
        /// <param name="pos">播放位置</param>
        /// <param name="vol">音量</param>
        /// <param name="pitch">音调</param>
        /// <param name="rolloffMode">距离渐变模式</param>
        /// <param name="minDist">最小距离</param>
        /// <param name="maxDist">最大距离</param>
        /// <returns></returns>
        public PooledAudioClip PlayClipAt(AudioClip clip, Vector3 pos, float vol = 1,
            float pitch = 1.0f, int rolloffMode = 1, float minDist = 10f, float maxDist = 100.0f)
        {
            PooledAudioClip pooledClip = SpawnTempAudio(pos, Quaternion.identity);
            AudioSource aSource = pooledClip.ASource;

            aSource.clip = clip;
            //设置其他项
            aSource.volume = vol * AudioSvc.AudioSetting.ClipVolume[_key];
            aSource.minDistance = minDist;
            aSource.maxDistance = maxDist;
            aSource.rolloffMode = (AudioRolloffMode)rolloffMode;
            aSource.pitch = pitch;


            pooledClip.Play(vol, _loopAudio);
            ActivedSource.Add(pooledClip);
            return pooledClip;
        }

        public PooledAudioClip PlayVoiceClip(AudioClip clip, Vector3 pos, float vol = 1,
            float pitch = 1.0f, float blend = 0, int rolloffMode = 1, float minDist = 10f, float maxDist = 100.0f,
            float lowPassFilter = 5500, float highPassFilter = 50, float distortionFilter = 0)
        {
            PooledAudioClip pooledClip = SpawnTempAudio(pos, Quaternion.identity);
            AudioSource aSource = pooledClip.ASource;

            aSource.clip = clip;
            //设置其他项
            aSource.volume = vol * AudioSvc.AudioSetting.ClipVolume[_key];
            aSource.spatialBlend = blend;
            aSource.rolloffMode = (AudioRolloffMode)rolloffMode;
            aSource.pitch = pitch;
            aSource.minDistance = minDist;
            aSource.maxDistance = maxDist;

            //目前架构不考虑Get不到的情况
            aSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = lowPassFilter;
            aSource.GetComponent<AudioHighPassFilter>().cutoffFrequency = highPassFilter;
            aSource.GetComponent<AudioDistortionFilter>().distortionLevel = distortionFilter;

            pooledClip.Play(vol, _loopAudio);
            ActivedSource.Add(pooledClip);
            return pooledClip;
        }

        PooledAudioClip SpawnTempAudio(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            PooledAudioClip tempAudio;
            if (pooledAudio.Count >= nextActive + 1 && !pooledAudio[nextActive].gameObject.activeSelf)  //下一个已经准备就绪 => 使用这个
            {
                tempAudio = pooledAudio[nextActive];
                tempAudio.gameObject.SetActive(true);
                tempAudio.transform.position = spawnPosition;
                tempAudio.transform.rotation = spawnRotation;
            }
            else                                     //下一个还没有准备好 => 插一个新的
            {
                tempAudio = NewItem();//实例化一个池中资源
                pooledAudio.Insert(nextActive, tempAudio);

                tempAudio = pooledAudio[nextActive];
                tempAudio.gameObject.SetActive(true);

                tempAudio.transform.position = spawnPosition;
                tempAudio.transform.rotation = spawnRotation;
            }

            if (nextActive == pooledAudio.Count - 1)
            {
                nextActive = 0;
            }
            else
            {
                nextActive++;
            }

            return tempAudio;
        }

        /// <summary>
        /// 从激活的列表中移除播完的音频
        /// </summary>
        /// <param name="source"></param>
        public void DeActiveClip(PooledAudioClip source)
        {
            ActivedSource.Remove(source);
        }
    }
}