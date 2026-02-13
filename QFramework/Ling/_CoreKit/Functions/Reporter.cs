using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QFramework
{
    public class Reporter : MonoBehaviour
    {
        public static System.Exception ReportEnum(Enum @enum)
        {
            throw new Exception("无效的枚举类型：" + @enum.ToString());
        }
    }
}