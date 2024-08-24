using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SequenceExState : StateMachineBehaviour
{
    public string paramter;
    public int[] weights;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DoRandomAction(animator);
    }

    private void DoRandomAction(Animator animator)
    {
        int total = 0;
        int idx = 0;
        // var ts = animator.GetCurrentAnimatorClipInfo(0);
        // var t1 = ts[0];

        foreach (int w in weights)
        {
            total += w;
        }


        int val = Random.Range(0, total);
        for (int i = 0; i < weights.Length; i++)
        {
            if (val < weights[i])
            {
                idx = i;
                break;
            }

            val -= weights[i];
        }

        if (!string.IsNullOrEmpty(paramter))
        {
            animator.SetInteger(paramter, idx);
        }
    }
}
