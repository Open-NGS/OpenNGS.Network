using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenNGS
{
    public static class MonoBehaviourExtension
    {
        //
        // 摘要:
        //     Starts a Coroutine.
        //
        // 参数:
        //   routine:
        public static OpenNGSCoroutineSequence StartCoroutineSequence(this MonoBehaviour behaviour, string sequenceName,params IEnumerator[] coroutines)
        {
            if (coroutines == null)
            {
                throw new NullReferenceException("routine is null");
            }

            OpenNGSCoroutineSequence sequence = new OpenNGSCoroutineSequence(sequenceName);
            foreach (var co in coroutines)
            {
                sequence.AddCoroutine(co);
            }
            behaviour.StartCoroutine(sequence.Run());
            return sequence;
        }

    }
}
