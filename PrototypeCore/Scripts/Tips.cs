using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class Tips : MonoBehaviour
    {
        [TextArea(5,20)]
        [HideLabel]
        public string TipsStr;
    }
}