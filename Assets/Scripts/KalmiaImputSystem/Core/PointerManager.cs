using UnityEngine;

namespace KIS.Core
{
    ///<summary>
    /// 手の座標・状態を統一的に管理(PalmDataManagerの後継)
    ///</summary>
    public class PointerManager
    {
        public Vector3 LeftPos { get; private set; }
        public Vector3 RightPos { get; private set; }
        public bool LeftGrab { get; private set; }
        public bool RightGrab { get; private set; }

        private const float smooth = 0.3f;

        public void UpdateLeft(Vector3 newpos,bool grab)
        {
            LeftPos = Vector3.Lerp(LeftPos, newpos, smooth);
            LeftGrab = grab;
        }

        public void UpdateRight(Vector3 newpos,bool grab)
        {
            RightPos = Vector3.Lerp(RightPos, newpos, smooth);
            RightGrab = grab;
        }

    }
}