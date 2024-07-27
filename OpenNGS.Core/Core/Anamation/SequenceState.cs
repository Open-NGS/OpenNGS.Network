using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceState : StateMachineBehaviour
{
    public string paramter;
    public int[] weights;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int total = 0;
        int idx = 0;
        foreach (int w in weights)
        {
            total += w;
        }
        

        int val = Random.Range(0, total);
        for (int i = 0; i < weights.Length; i++)
        {
            if (val <= weights[i])
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
