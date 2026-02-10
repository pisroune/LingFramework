using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IAudioSetting
    {
        Dictionary<int, float> SourceVolume { get; set; }
        Dictionary<int, float> ClipVolume { get; set; }
    }
}