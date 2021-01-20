using UnityEngine;
public class PlayerAnimationHash
{
    public static int Idle = Animator.StringToHash("Base Layer.Player_Idle");
    public static int Walk = Animator.StringToHash("Base Layer.Player_Walk");
    public static int Run = Animator.StringToHash("Base Layer.Player_Run");
    public static int Jump = Animator.StringToHash("Base Layer.Player_Jump");
    public static int AttackA = Animator.StringToHash("Base Layer.Player_Attack_A");
    public static int AttackB = Animator.StringToHash("Base Layer.Player_Attack_B");
    public static int AttackC = Animator.StringToHash("Base Layer.Player_Attack_C");
    public static int JumpAttack = Animator.StringToHash("Base Layer.Player_JumpAttack");
}
