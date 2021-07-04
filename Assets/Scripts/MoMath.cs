using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoMath
{
    public enum Direction
    {
        Front,
        Back,
        Right,
        Left
    }

    // XZ平面を上から見たときの方向、Z+方向が上、X+方向が右である
    public enum XZDirection
    {
        UP_ZPlus = 0,
        DOWN_ZMinus,
        RIGHT_XPlus,
        LEFT_XMinus,
    }

    // 全関数をクラスの管理に置かなくてもいいでしょうC#さん(´；ω；｀)
    class DirectionMath
    {
        public static XZDirection FromDegree(float degree)
        {
            float calcDegree = degree;
            if (degree < 0.0f) { calcDegree += 360.0f; }

            if (calcDegree >= 315.0f || calcDegree < 45.0f)
            {
                return XZDirection.UP_ZPlus;
            }
            else if (calcDegree < 135.0f)
            {
                // if (45.0f <= calcDegree && calcDegree < 135.0f)
                return XZDirection.RIGHT_XPlus;
            }
            else if (calcDegree < 225.0f)
            {
                // if (135.0f <= calcDegree && calcDegree < 225.0f)
                return XZDirection.DOWN_ZMinus;
            }
            else
            {
                // if (225 <= calcDegree && calcDegree < 315.0f)だけどもう他これしかないのでelseで
                return XZDirection.LEFT_XMinus;
            }

        }

        public static Vector3 EulerFromDirection(XZDirection direction)
        {
            switch (direction)
            {
                case XZDirection.UP_ZPlus:
                    return new Vector3(0, 0, 0);
                case XZDirection.DOWN_ZMinus:
                    return new Vector3(0, 180, 0);
                case XZDirection.RIGHT_XPlus:
                    return new Vector3(0, 90, 0);
                case XZDirection.LEFT_XMinus:
                    return new Vector3(0, 270, 0);
                default:
                    return new Vector3(0, 0, 0);
            }
        }

        public static Vector3 FromDirection(XZDirection direction)
        {
            switch (direction)
            {
                case XZDirection.UP_ZPlus:
                    return Vector3.forward;
                case XZDirection.DOWN_ZMinus:
                    return Vector3.back;
                case XZDirection.RIGHT_XPlus:
                    return Vector3.right;
                case XZDirection.LEFT_XMinus:
                    return Vector3.left;
                default:
                    return Vector3.forward;
            }
        }

        public static XZDirection Inverse(XZDirection direction)
        {
            switch (direction)
            {
                case XZDirection.UP_ZPlus:
                    return XZDirection.DOWN_ZMinus;
                case XZDirection.DOWN_ZMinus:
                    return XZDirection.UP_ZPlus;
                case XZDirection.RIGHT_XPlus:
                    return XZDirection.LEFT_XMinus;
                case XZDirection.LEFT_XMinus:
                    return XZDirection.RIGHT_XPlus;
                default:
                    return XZDirection.UP_ZPlus;
            }

        }
    }

    class LerpMath
    {
        // 2次ベジェ曲線
        public static Vector3 QuadraticBeziercurve(Vector3 start, Vector3 controlPoint, Vector3 end, float t)
        {
            Vector3 ret = new Vector3(0, 0, 0);

            // fromからcontrolをLerpで移動する点
            Vector3 startToControl = Vector3.Lerp(start, controlPoint, t);
            // controlからtoをLerpで移動する点
            Vector3 controlToEnd = Vector3.Lerp(controlPoint, end, t);

            // 最終的に求める点は、上二つの点を結ぶ線上をLerpで移動する点
            ret = Vector3.Lerp(startToControl, controlToEnd, t);

            return ret;
        }
    }

}