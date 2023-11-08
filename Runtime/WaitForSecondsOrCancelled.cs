using System.Collections;
using UnityEngine;

namespace OrderedActionSequences
{
    public static class Wait
    {
        public static IEnumerator ForSecondsOrCancelled(float seconds, ICancelToken token)
        {
            float time = 0;
            while (time <= seconds && !token.IsCancelled)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
