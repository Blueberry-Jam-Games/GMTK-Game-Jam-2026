using System.Collections;
using BJ;
using UnityEngine;

public class IntroTransition : LevelTransitionEffect
{
    [SerializeField]
    private Animator animator;

    public override IEnumerator CurtainsDown()
    {
        yield break;
    }

    public override IEnumerator CurtainsUp()
    {
        animator.Play("IntroDialog");
        yield return new WaitForSeconds(4.25f);
    }

    public override void JumpToCurtainsDown()
    {
        // skip
    }

    public override void JumpToCurtainsUp()
    {
        // skip
    }

    public override IEnumerator UpdateProgress(double progress)
    {
        yield break;
    }
}
