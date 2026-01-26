using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class MathMethod
    {
        /// <summary>
        /// 平滑改变数值（每帧改变smooth这么多）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetValue"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public static float Slerp(float value, float targetValue, float smooth)
        {
            if (targetValue > value)
            {
                if (value + smooth < targetValue)
                    value += smooth;
                else
                    value = targetValue;
            }
            else if (targetValue < value)
            {
                if (value - smooth > targetValue)
                {
                    value -= smooth;
                }
                else
                    value = targetValue;
            }

            return value;
        }

        public static Vector3 MoveTo(Vector3 self, Vector3 target, float speed)
        {
            Vector3 dir = (target - self).normalized;
            float tempDist = Vector3.Distance(self, target);
            if (tempDist < speed)
            {
                return target;
            }

            return self + dir * speed;
        }

        /// <summary>
        /// 求解一元二次方程
        /// ax^2 + bx + c = 0
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <param name="c">c</param>
        /// <param name="x1">第一个解</param>
        /// <param name="x2">第二个解</param>
        /// <returns></returns>
        public static bool GetEquation(float a, float b, float c, out float x1, out float x2)
        {
            float d = b * b - 4 * a * c;
            if (d > 0)
            {
                x1 = (-b - Mathf.Sqrt(d)) / 2.0f / a;
                x2 = (-b + Mathf.Sqrt(d)) / 2.0f / a;
                return true;
            }
            else if (d == 0)
            {
                x1 = x2 = (-b) / 2.0f / a;
                return true;
            }
            else
            {
                double i = Mathf.Sqrt(-d) / 2.0 / a;
                x1 = x2 = -b / 2.0f / a;
            }

            return false;
        }

        //随机正负号
        public static float RandomSign()
        {
            int p = UnityEngine.Random.Range(0, 2);
            if (GetPercent(50))
            {
                return 1;
            }

            return -1;
        }

        public static Vector3 IgnoreY(Vector3 v3)
        {
            return new Vector3(v3.x, 0, v3.z);
        }

        /// <summary>
        /// 法线（平行于xz面）
        /// </summary>
        /// <param name="v3">当前向量</param>
        /// <returns>法线向量</returns>
        public static Vector3 Normal(Vector3 v3)
        {
            Vector3 normalLine;
            if (v3 != Vector3.zero)
            {
                normalLine = new Vector3(-v3.z, 0, v3.x);
                //normalLine = new Vector3(-v3.z, v3.y, v3.x);
                return normalLine;
            }

            return Vector3.zero;
        }


        /// 点point到线段x1-x2的距离（xz平面）
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p0"></param>
        /// <returns></returns>
        public static float GetDisFromPointToLine(Vector3 x1, Vector3 x2, Vector3 point)
        {
            float space = 0;
            float a, b, c;
            a = XZDistance(x1, x2); // lineSpace(x1, y1, x2, y2);// 线段的长度    
            b = XZDistance(x1, point); // (x1,y1)到点的距离    
            c = XZDistance(x2, point); // (x2,y2)到点的距离    
            if (c <= 0.001 || b <= 0.001)
            {
                space = 0;
                return space;
            }

            if (a <= 0.001)
            {
                space = b;
                return space;
            }

            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }

            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }

            float p = (a + b + c) / 2; // 半周长    
            float s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c)); // 海伦公式求面积    
            space = 2 * s / a; // 返回点到线的距离（利用三角形面积公式求高）    
            return space;
        }

        /// <summary>
        /// 求point到线 begin-end 的垂足点坐标（xz平面）
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="begin">线坐标1</param>
        /// <param name="end">线坐标2</param>
        /// <returns>垂足位置</returns>
        public static Vector3 GetFootOfPerpendicular(Vector3 point, Vector3 begin, Vector3 end)
        {
            float dx = begin.x - end.x;
            float dz = begin.z - end.z;
            if (Mathf.Abs(dx) < 0.0001 && Mathf.Abs(dz) < 0.0001)
            {
                return begin;
            }

            float u = (point.x - begin.x) * (begin.x - end.x) +
                      (point.z - begin.z) * (begin.z - end.z);
            u = u / ((dx * dx) + (dz * dz));
            //retVal.X = begin.X + u* dx;
            //   retVal.Y = begin.Y + u* dy;

            Vector3 retVal = new Vector3(begin.x + u * dx, begin.y, begin.z + u * dz);
            return retVal;
        }


        /// <summary>
        /// 向量AP在向量AB上的投影长度
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointP"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float GetProjectionLength(Vector3 pointA, Vector3 pointB, Vector3 pointP)
        {
            // 向量AB
            Vector3 AB = pointB - pointA;
            // 向量AP
            Vector3 AP = pointP - pointA;

            // 检查AB是否为零向量
            if (AB.sqrMagnitude == 0)
            {
                throw new ArgumentException("pointA and pointB cannot be the same point.");
            }

            // 计算投影长度
            float projectionLength = Vector3.Dot(AP, AB) / AB.magnitude;

            return projectionLength;
        }

        /// <summary>
        /// 根据投影距离对 MonoBehaviour 对象列表进行排序，并返回按升序排序的列表。
        /// </summary>
        /// <param name="pointA">用于定义投影线的起始点。</param>
        /// <param name="pointB">用于定义投影线的终止点。</param>
        /// <param name="list">需要排序的 MonoBehaviour 对象列表。</param>
        /// <typeparam name="T">列表中对象的类型，必须继承自 MonoBehaviour。</typeparam>
        /// <returns>按投影距离升序排序后的 MonoBehaviour 对象列表。</returns>
        /// <exception cref="ArgumentException">当点 pointA 和 pointB 相同时抛出异常，因为此时无法定义有效的投影线。</exception>
        public static List<T> SortByProjectionDistance<T>(Vector3 pointA, Vector3 pointB, List<T> list)
            where T : MonoBehaviour
        {
            // 计算单位向量 AB
            Vector3 unitAB = (pointB - pointA).normalized;

            // 检查点 A 和 B 是否相同，避免归一化零向量问题
            if (unitAB == Vector3.zero)
            {
                throw new ArgumentException("Point A and Point B cannot be the same.");
            }

            // 按投影距离排序
            return list.OrderBy(P =>
            {
                Vector3 AP = P.transform.position - pointA; // 获取对象的位置
                return Vector3.Dot(unitAB, AP); // 计算投影距离
            }).ToList();
        }

        /// <summary>
        /// 根据投影距离对 MonoBehaviour 对象列表进行排序，并返回按升序排序的距离列表。
        /// </summary>
        /// <param name="pointA">用于定义投影线的起始点。</param>
        /// <param name="pointB">用于定义投影线的终止点。</param>
        /// <param name="list">需要排序的 MonoBehaviour 对象列表。</param>
        /// <typeparam name="T">列表中对象的类型，必须继承自 MonoBehaviour。</typeparam>
        /// <returns>按投影距离升序排序后的 MonoBehaviour 对象列表。</returns>
        /// <exception cref="ArgumentException">当点 pointA 和 pointB 相同时抛出异常，因为此时无法定义有效的投影线。</exception>
        public static List<(T, int)> SortWithProjectionDistance<T>(Vector3 pointA, Vector3 pointB, List<T> list)
            where T : MonoBehaviour
        {
            List<(T, int)> res = new List<(T, int)>();
            // 计算单位向量 AB
            Vector3 unitAB = (pointB - pointA).normalized;

            // 检查点 A 和 B 是否相同，避免归一化零向量问题
            if (unitAB == Vector3.zero)
            {
                throw new ArgumentException("Point A and Point B cannot be the same.");
            }

            foreach (var obj in list)
            {
                float distance = Vector3.Dot(unitAB, obj.transform.position - pointA);
                res.Add((obj, (int)distance));
            }

            // 按投影距离排序
            return res.OrderBy(tuple => tuple.Item2).ToList();
        }

        public static List<(T, int)> SortWithDistance<T>(Vector3 pointA, List<T> list)
            where T : MonoBehaviour
        {
            List<(T, int)> res = new List<(T, int)>();
            foreach (var obj in list)
            {
                res.Add((obj, (int)XZDistance(pointA, obj.transform.position)));
            }

            return res.OrderBy(tuple => tuple.Item2).ToList();
        }


        /// <summary>
        /// 返回A、B的平均值
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns></returns>
        public static Vector3 Middle(Vector3 vectorA, Vector3 vectorB)
        {
            return (vectorA + vectorB) / 2;
        }

        /// <summary>
        /// 得到两个位置相对self的夹角
        /// </summary>
        /// <param name="self">默认位置</param>
        /// <param name="targetA">目标1</param>
        /// <param name="targetB">目标2</param>
        /// <param name="n">垂直向量（通常Vector3.up）</param>
        /// <returns></returns>
        public static int GetAngleInt(Vector3 self, Vector3 targetA, Vector3 targetB, Vector3 n)
        {
            Vector3 directionA = targetA - self;
            Vector3 directionB = targetB - self;
            return IntDegree(directionA, directionB, n);
        }

        /// <summary>
        /// 求得两向量夹角（degree）整形，带正负
        /// </summary>
        /// <param name="v1">向量1</param>
        /// <param name="v2">向量2</param>
        /// <param name="n">垂直向量（通常Vector3.up）</param>
        /// <returns>夹角</returns>
        public static int IntDegreeSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            float angle = Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
            return (int)angle;
        }

        /// <summary>
        /// 求得两向量夹角（degree）整形，不带正负
        /// </summary>
        /// <param name="v1">向量1</param>
        /// <param name="v2">向量2</param>
        /// <param name="n">垂直向量（通常Vector3.up）</param>
        /// <returns></returns>
        public static int IntDegree(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Abs(IntDegreeSigned(v1, v2, n));
        }

        /// <summary>
        /// 求得两向量夹角（degree）浮点型，带正负
        /// </summary>
        /// <param name="v1">向量1</param>
        /// <param name="v2">向量2</param>
        /// <param name="n">垂直向量（通常Vector3.up）</param>
        /// <returns>夹角</returns>
        public static float FloDegreeSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 求得两向量夹角（degree）浮点型，不带正负
        /// </summary>
        /// <param name="v1">向量1</param>
        /// <param name="v2">向量2</param>
        /// <param name="n">垂直向量（通常Vector3.up）</param>
        /// <returns></returns>
        public static float FloDegree(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Abs(FloDegreeSigned(v1, v2, n));
        }

        public static float GetRadian(float degree)
        {
            return degree * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 返回 ralative 相对于 local 本地坐标下的相对角度
        /// </summary>
        /// <param name="local"></param>
        /// <param name="ralative"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static float RalativeAngle(Transform local, Vector3 ralative, Vector3 n)
        {
            return FloDegreeSigned(local.forward, ralative - local.position, n);
        }

        //返回XZ平面上的距离，无视y轴（高度）
        public static float XZDistance(Transform transA, Transform transB)
        {
            return XZDistance(transA.position, transB.position);
        }

        public static float XZDistance(Vector3 posA, Vector3 posB)
        {
            float num = posA.x - posB.x;
            float num2 = posA.z - posB.z;
            return (float)Mathf.Sqrt(num * num + num2 * num2);
        }

        public static bool XZShorterThan(Vector3 posA, Vector3 posB, float distance)
        {
            float num = posA.x - posB.x;
            float num2 = posA.z - posB.z;
            return (num * num + num2 * num2) < distance * distance;
        }

        public static bool XZLongerThan(Vector3 posA, Vector3 posB, float distance)
        {
            float num = posA.x - posB.x;
            float num2 = posA.z - posB.z;
            return (num * num + num2 * num2) > distance * distance;
        }

        public static bool ShorterThan(Vector3 posA, Vector3 posB, float distance)
        {
            float num = posA.x - posB.x;
            float num2 = posA.y - posB.y;
            float num3 = posA.z - posB.z;
            return (num * num + num2 * num2 + num3 * num3) < distance * distance;
        }

        public static bool LongerThan(Vector3 posA, Vector3 posB, float distance)
        {
            float num = posA.x - posB.x;
            float num2 = posA.y - posB.y;
            float num3 = posA.z - posB.z;
            return (num * num + num2 * num2 + num3 * num3) > distance * distance;
        }

        public static float XZSqrtDistance(Vector3 posA, Vector3 posB)
        {
            float num = posA.x - posB.x;
            float num2 = posA.z - posB.z;
            return num * num + num2 * num2;
        }

        public static float SqrtDistance(Vector3 posA, Vector3 posB)
        {
            float num = posA.x - posB.x;
            float num2 = posA.y - posB.y;
            float num3 = posA.z - posB.z;
            return num * num + num2 * num2 + num3 * num3;
        }

        //返回to相对from在XZ平面上的方向，无视y轴
        public static Vector3 XZDirection(Vector3 from, Vector3 to)
        {
            return to - new Vector3(from.x, to.y, from.z);
        }

        public static Vector3 XZDirection(Transform from, Transform to)
        {
            return to.position - new Vector3(from.position.x, to.position.y, from.position.z);
        }

        //roll 一个 value 看是否大于 0-100中的随机数（int型）
        public static bool GetPercent(int value)
        {
            int v = UnityEngine.Random.Range(0, 100);
            if (value > v)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //roll 一个 value 看是否大于 0-100中的随机数（float型）
        public static bool GetPercent(float value)
        {
            float v = UnityEngine.Random.Range(0f, 1f);
            if (value > v)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 返回一个半径radius以内！的随机xz平面向量
        public static Vector3 ReturnRandomOffset(float radius)
        {
            if (radius == 0)
                return Vector3.zero;

            float radin = UnityEngine.Random.Range(0, 2 * Mathf.PI); //返回任意角度
            Vector3 v = new Vector3(UnityEngine.Random.Range(-radius, radius) * Mathf.Cos(radin), 0,
                UnityEngine.Random.Range(-radius, radius) * Mathf.Sin(radin));
            return v;
        }

        // 返回一个半径radius的随机xz平面向量
        public static Vector3 ReturnOffset(float radius)
        {
            if (radius == 0)
                return Vector3.zero;

            float radin = UnityEngine.Random.Range(0, 2 * Mathf.PI); //返回任意角度
            Vector3 v = new Vector3(radius * Mathf.Cos(radin), 0, radius * Mathf.Sin(radin));
            return v;
        }

        //int型转成时间 （xx:xx）
        public static string IntToTime(int integer)
        {
            if (integer < 0)
            {
                return "00:00";
            }

            int minute = integer / 60;
            int second = integer % 60;
            string minuteStr;
            string secondStr;
            if (minute < 10)
            {
                minuteStr = "0" + minute.ToString();
            }
            else
            {
                minuteStr = minute.ToString();
            }

            if (second < 10)
            {
                secondStr = "0" + second.ToString();
            }
            else
            {
                secondStr = second.ToString();
            }


            return minuteStr + ":" + secondStr;
        }

        /// <summary>
        /// 判断n是奇数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsOdd(int n)
        {
            return System.Convert.ToBoolean(n % 2);
        }
    }
}