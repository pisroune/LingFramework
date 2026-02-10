using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AudioRely
    {
        PooledAudioClip _clip;
        Transform _relyTrans;

        Action<AudioRely> _onStop;

        public AudioRely(PooledAudioClip clip, Transform relyTrans, Action<AudioRely> onStop)
        {
            _clip = clip;
            _relyTrans = relyTrans;
            _onStop = onStop;
        }

        public void Update()
        {
            if (!_clip.gameObject.activeSelf)
            {
                _onStop?.Invoke(this);
                _onStop = null;
                return;
            }

            if (_relyTrans != null && _relyTrans.IsActive())
            {  //¸úËæ
                _clip.transform.position = _relyTrans.transform.position;
            }
            else
            {  //½¥±äÒÆ³ý
                _clip.StopAudio();
                _onStop?.Invoke(this);
                _onStop = null;
            }
        }
    }
}