using UnityEngine;

public class AirAttackUpState : PlayerState
{
    public AirAttackUpState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("AirAttackUp"); // Khớp với Parameter trong Animator của bạn
        player.attackPoint.localPosition = player.airUpPos;
        player.currentHitboxSize = player.airUpSize;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.IsGrounded()) stateMachine.ChangeState(player.groundedState);
    }

    public override void AnimationFinishTrigger() => stateMachine.ChangeState(player.airState);
}