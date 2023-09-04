using UnityEngine;

namespace Core.Utility
{
    public class DestroyThisObject : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject, stateInfo.length);
        }
    }
}
