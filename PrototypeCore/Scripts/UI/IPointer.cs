using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public interface IPointer
    {
        void PointerEnter();
        void PointerExit();
        void PointerStay();
    }
}