using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModdingUtils.Extensions;

namespace NoireWallCards
{
    internal class FixedOutOfBoundsHelpers
    {
        internal static bool SkipEmbigBouncePatch = false;
        internal static Transform Border = null;
        internal static RectTransform BorderRect
        {
            get
            {
                if(Border.childCount == 0)
                {
                    return Border.GetComponent<RectTransform>();
                }
                return Border.GetChild(0).GetComponent<RectTransform>();
            }
        }

        internal static Vector3 GetPoint(Vector3 p)
        {
            return Vector3.zero;
            /*float x = Mathf.Lerp(-35.56f, 35.56f, p.x);
            float y = Mathf.Lerp(-20f, 20f, p.y);
            return new Vector3(x, y, 0f);*/
        }

        internal static Vector3 BoundsPointFromWorldPosition(OutOfBoundsHandler oob, Vector3 worldPosition)
        {
            Vector3 min = RotateFromBoundsFrame(oob.WorldPositionFromBoundsPoint(Vector3.zero));
            Vector3 max = RotateFromBoundsFrame(oob.WorldPositionFromBoundsPoint(Vector3.one));

            Vector3 rotated = RotateFromBoundsFrame(worldPosition);

            return new Vector3(Mathf.InverseLerp(min.x, max.x, rotated.x), Mathf.InverseLerp(min.y, max.y, rotated.y), 0f);
        }

        private static Vector3 RotateFromBoundsFrame(Vector3 point)
        {

            float cos = Mathf.Cos(Mathf.PI * BorderRect.rotation.eulerAngles.z / 180f);
            float sin = Mathf.Sin(Mathf.PI * BorderRect.rotation.eulerAngles.z / 180f);
            return new Vector3(cos * point.x + sin * point.y, -sin * point.x + cos * point.y, point.z);
        }
    }
}
