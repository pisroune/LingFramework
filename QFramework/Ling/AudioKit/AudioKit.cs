using Prototype;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AudioSettingModel : IAudioSetting
    {
        public Dictionary<int, float> SourceVolume { get; set; }
        public Dictionary<int, float> ClipVolume { get; set; }

        public AudioSettingModel()
        {
            SourceVolume = new Dictionary<int, float>();
            ClipVolume = new Dictionary<int, float>();
            SourceVolume.Add(1, 0);
            SourceVolume.Add(2, 0);
            SourceVolume.Add(3, 0);

            ClipVolume.Add(1, 0);
            ClipVolume.Add(2, 0);
            ClipVolume.Add(3, 0);
            ClipVolume.Add(4, 0);
            ClipVolume.Add(5, 1);
        }
    }

    public class AudioModel
    {

    }


    public class AudioKit : MonoBehaviour
    {
        public enum BGMType
        {
            FadingIn,
            FadingOut,
            Playing,
            Stop
        }
        static AudioClip[] _buttonClips;
        static BGMType _bgmT;
        static float _fadeTime;

        static float _localMusicVolume;
        static float _localSfxVolume;
        static float _localEnvVolume;
        static AudioSettingModel _audioSetting;

        static float OuterMainVolume = 1;
        static float OuterMusicVolume = 1;
        static float OuterSfxVolume = 1;
        static float OuterEnvVolume = 1;
        static float OuterVoiceVolume = 1;

        public static void SetOuterMainVolume(float volume)
        {
            OuterMainVolume = volume;
            _audioSetting.SourceVolume[1] = MusicVolume;   //BGM播放器    单例
            _audioSetting.SourceVolume[2] = SfxVolume;   //UI音效播放器 单例
            _audioSetting.SourceVolume[3] = EnvVolume;     //UI音效播放器 单例

            _audioSetting.ClipVolume[1] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[2] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[3] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[4] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[5] = OuterMainVolume * OuterVoiceVolume;     //音效播放器   资源池
            AudioSvc.PooledListDict[1].UpdateVolume();
            AudioSvc.PooledListDict[2].UpdateVolume();
            AudioSvc.PooledListDict[3].UpdateVolume();
            AudioSvc.PooledListDict[4].UpdateVolume();
            AudioSvc.PooledListDict[5].UpdateVolume();
        }
        public static void SetOuterMusicVolume(float volume)
        {
            OuterMusicVolume = volume;
            _audioSetting.SourceVolume[1] = MusicVolume;
        }
        public static void SetOuterSfxVolume(float volume)
        {
            OuterSfxVolume = volume;
            _audioSetting.SourceVolume[2] = SfxVolume;   //UI音效播放器 单例
            _audioSetting.ClipVolume[1] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[2] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[3] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[4] = OuterMainVolume * OuterSfxVolume;     //音效播放器   资源池
            AudioSvc.PooledListDict[1].UpdateVolume();
            AudioSvc.PooledListDict[2].UpdateVolume();
            AudioSvc.PooledListDict[3].UpdateVolume();
            AudioSvc.PooledListDict[4].UpdateVolume();
        }
        public static void SetOuterEnvVolume(float volume)
        {
            OuterEnvVolume = volume;
            _audioSetting.SourceVolume[3] = EnvVolume;   //BGM播放器
        }
        public static void SetOuterVoiceVolume(float volume)
        {
            OuterVoiceVolume = volume;
            _audioSetting.ClipVolume[5] = OuterMainVolume * OuterVoiceVolume;     //音效播放器   资源池
            AudioSvc.PooledListDict[5].UpdateVolume();
        }

        static float MusicVolume => _localMusicVolume * OuterMusicVolume;
        static float SfxVolume => _localSfxVolume * OuterSfxVolume;
        static float EnvVolume => _localEnvVolume * OuterEnvVolume;

        public static void Init()
        {
            AudioSvc.Init();
            _audioSetting = new AudioSettingModel();

            _audioSetting.SourceVolume[1] = MusicVolume;   //BGM播放器    单例
            _audioSetting.SourceVolume[2] = SfxVolume;     //UI音效播放器 单例
            _audioSetting.SourceVolume[3] = EnvVolume;     //UI音效播放器 单例

            _audioSetting.ClipVolume[1] = OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[2] = OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[3] = OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[4] = OuterSfxVolume;     //音效播放器   资源池
            _audioSetting.ClipVolume[5] = OuterVoiceVolume;     //音效播放器   资源池

            AudioSvc.AudioSetting = _audioSetting;

            AudioSvc.InitClipListItem(1, 30, 1, false);
            AudioSvc.InitClipListItem(2, 5, 1, true);
            AudioSvc.InitClipListItem(3, 5, 0, false);
            AudioSvc.InitClipListItem(4, 5, 0, true);
            AudioSvc.InitVoiceListItem(5, 5, 0, false);
        }

        public void Update()
        {
            switch (_bgmT)
            {
                case BGMType.FadingIn:
                    float rate = _fadeTime == 0 ? 100 : 1 / _fadeTime;
                    _audioSetting.SourceVolume[1] += rate * MusicVolume * Time.deltaTime;
                    if (_audioSetting.SourceVolume[1] >= MusicVolume)
                    {
                        _audioSetting.SourceVolume[1] = MusicVolume;
                        _bgmT = BGMType.Playing;
                    }
                    break;
                case BGMType.FadingOut:
                    rate = _fadeTime == 0 ? 100 : 1 / _fadeTime;
                    _audioSetting.SourceVolume[1] -= rate * MusicVolume * Time.deltaTime;
                    if (_audioSetting.SourceVolume[1] <= 0)
                    {
                        _audioSetting.SourceVolume[1] = 0;
                        _bgmT = BGMType.Stop;
                        AudioSvc.StopAudioSource(1);
                    }
                    break;
                default:
                case BGMType.Playing:
                case BGMType.Stop:
                    break;
            }
        }

        public static AudioSource PlayBGM(AudioClip clip, bool loop, float volume = 1, float fadeTime = 0)
        {
            if (clip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            _localMusicVolume = volume;
            _audioSetting.SourceVolume[1] = 0;   //BGM播放器    单例
            _fadeTime = fadeTime;
            _bgmT = BGMType.FadingIn;
            return AudioSvc.PlayAudioSource(1, clip, loop, 0, true);
        }
        public static void StopBGM(float fadeTime = 0)
        {
            _fadeTime = fadeTime;
            _bgmT = BGMType.FadingOut;
        }
        public static AudioSource PlayEffect(AudioClip clip, float volume = 1)
        {
            if (clip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            _localSfxVolume = volume;
            _audioSetting.SourceVolume[2] = SfxVolume;   //BGM播放器    单例

            return AudioSvc.PlayAudioSource(2, clip, false, SfxVolume);
        }
        public static AudioSource PlayEnv(AudioClip clip, float volume = 1, float lowPassFilter = 6000, float highPassFilter = 50)
        {
            if (clip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            _localEnvVolume = volume;
            _audioSetting.SourceVolume[3] = EnvVolume;   //BGM播放器    单例

            AudioSource aSource = AudioSvc.PlayAudioSource(3, clip, true, EnvVolume, true);
            AudioSvc.MakeSureLowPassFilter(3, lowPassFilter);
            AudioSvc.MakeSureHighPassFilter(3, highPassFilter);
            return aSource;
        }
        public static void StopEnv()
        {
            AudioSvc.StopAudioSource(3);
        }

        public static PooledAudioClip PlayClipAt(AudioClip audioClip, Vector3 startPosition, float vol = 1,
            float pitch = 1.0f, int rolloffMode = 1, float minDist = 10f, float maxDist = 100.0f)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(1, audioClip, startPosition, vol, pitch, rolloffMode, minDist, maxDist);
        }
        public static PooledAudioClip PlayLoopClipAt(AudioClip audioClip, Vector3 startPosition, float vol = 1,
            float pitch = 1.0f, int rolloffMode = 1, float minDist = 10f, float maxDist = 100.0f)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(2, audioClip, startPosition, vol, pitch, rolloffMode, minDist, maxDist);
        }

        public static PooledAudioClip Play2DClip(AudioClip audioClip, float vol = 1, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(3, audioClip, Vector3.zero, vol, pitch, 0, 1, 1000);
        }
        /// <summary>
        /// 输入路径
        /// 文件必须放在ResAudio文件下，输入路径时省略ResAudio
        /// </summary>
        /// <param name="path"></param>
        /// <param name="vol"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public static PooledAudioClip Play2DClip(string path, float vol = 1, float pitch = 1.0f)
        {
            AudioClip clip = AssetManager.LoadAudio(path);
            if (clip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(3, clip, Vector3.zero, vol, pitch);
        }

        public static PooledAudioClip Play2DLoopClip(AudioClip audioClip, float vol = 1, float pitch = 1.0f)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(4, audioClip, Vector3.zero, vol, pitch);
        }
        public static PooledAudioClip Play2DLoopClip(string path, float vol = 1, float pitch = 1.0f)
        {
            AudioClip clip = AssetManager.LoadAudio(path);
            if (clip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayClipAt(4, clip, Vector3.zero, vol, pitch);
        }

        #region 人声相关方法

        public enum AudioSpeakType
        {
            [LabelText("指挥官电台（清晰）")]
            Commander,
            [LabelText("载具电台（嘈杂）")]
            Chariot,
            [LabelText("士兵")]
            Soldier,
            [LabelText("远方士兵")]
            FarSoldier
        }

        public static PooledAudioClip Play2DVoiceClip(AudioClip audioClip, AudioSpeakType speakT, float vol = 1, float pitch = 1.0f)
        {
            switch (speakT)
            {
                default:
                case AudioSpeakType.Commander:
                    return Play2DVoiceClip(audioClip, vol, pitch, 6000, 500, 0.4f);
                case AudioSpeakType.Chariot:
                    return Play2DVoiceClip(audioClip, vol, pitch, 3500, 2000, 0.7f);
                case AudioSpeakType.Soldier:
                    return Play2DVoiceClip(audioClip, vol, pitch, 5000, 250, 0);
                case AudioSpeakType.FarSoldier:
                    return Play2DVoiceClip(audioClip, vol, pitch, 3500, 250, 0);
            }
        }
        public static PooledAudioClip PlayVoiceClipAt(AudioClip audioClip, Vector3 position, AudioSpeakType speakT, float vol = 1, float pitch = 1.0f)
        {
            switch (speakT)
            {
                default:
                case AudioSpeakType.Commander:
                    return PlayVoiceClipAt(audioClip, position, vol, pitch, 5, 100, 6000, 1000, 0.2f);
                case AudioSpeakType.Chariot:
                    return PlayVoiceClipAt(audioClip, position, vol, pitch, 5, 100, 6000, 5000, 0.6f);
                case AudioSpeakType.Soldier:
                    return PlayVoiceClipAt(audioClip, position, vol, pitch, 5, 100, 4000, 500, 0);
                case AudioSpeakType.FarSoldier:
                    return PlayVoiceClipAt(audioClip, position, vol, pitch, 5, 100, 3000, 500, 0);
            }
        }


        /// <summary>
        /// 人声播放
        /// 旁白、简报
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="vol"></param>
        /// <param name="pitch"></param>
        /// <param name="blend2D"></param>
        /// <param name="lowPassFilter"></param>
        /// <param name="highPassFilter"></param>
        /// <param name="distortionFilter"></param>
        /// <returns></returns>
        public static PooledAudioClip Play2DVoiceClip(AudioClip audioClip, float vol = 1, float pitch = 1.0f, float lowPassFilter = 5500, float highPassFilter = 50, float distortionFilter = 0)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayVoiceClipAt(5, audioClip, Vector3.zero, vol, pitch, 0, 1, 10, 100, lowPassFilter, highPassFilter, distortionFilter);
        }

        /// <summary>
        /// 人声播放
        /// 喊话、场景对白
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="vol"></param>
        /// <param name="pitch"></param>
        /// <param name="blend2D"></param>
        /// <param name="lowPassFilter"></param>
        /// <param name="highPassFilter"></param>
        /// <param name="distortionFilter"></param>
        /// <returns></returns>
        public static PooledAudioClip PlayVoiceClipAt(AudioClip audioClip, Vector3 startPosition, float vol = 1, float pitch = 1.0f,
            float minDist = 10, float maxDist = 100, float lowPassFilter = 5500, float highPassFilter = 50, float distortionFilter = 0)
        {
            if (audioClip == null)
            {
                throw new Exception("No Audio Clip!");
            }
            return AudioSvc.PlayVoiceClipAt(5, audioClip, startPosition, vol, pitch, 1, 1, minDist, maxDist, lowPassFilter, highPassFilter, distortionFilter);
        }

        #endregion

        public static void AddRely(PooledAudioClip clip, Transform relyTrans)
        {
            AudioSvc.AddRely(clip, relyTrans);
        }
    }
}