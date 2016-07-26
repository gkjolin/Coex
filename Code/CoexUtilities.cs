using UnityEngine;

namespace IFGame.Lix
{
    public class WaitForSeconds : YieldInstruction
    {
        float mSeconds;
        float mTimeout;
        bool mIgnoreTimescale;

        internal bool timeout
        {
            get
            {
                return mIgnoreTimescale 
                    ? Time.unscaledTime >= mTimeout 
                    : Time.time >= mTimeout;
            }
        }

        public WaitForSeconds(float seconds, bool ignoreTimescale = false)
        { 
            mIgnoreTimescale = ignoreTimescale;
            mSeconds = seconds;
        }

        internal void Reset()
        {
            mTimeout = mIgnoreTimescale ? Time.unscaledTime + mSeconds : Time.time + mSeconds;
        }
    }

    public class WaitForEndOfFrame : YieldInstruction { }

    public class WaitForFixedUpdate : YieldInstruction { }
}