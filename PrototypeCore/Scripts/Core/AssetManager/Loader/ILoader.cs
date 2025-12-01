using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Prototype
{
    public interface ILoader
    {
        GameObject LoadPrefab(string path, bool cache = false);

        Sprite LoadSprite(string path, bool cache = false);

        Texture LoadTexture(string path, bool cache = false);

        AudioClip LoadAudio(string path, bool cache = false);

        XmlDocument LoadXml(string path);

        T Load<T>(string path) where T : UnityEngine.Object;

        T[] LoadAll<T>(string path) where T : UnityEngine.Object;
    }
}