using UnityEngine;

public class AirAttackForwardState : PlayerState
{
    public AirAttackForwardState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("AirAttackForward");
        player.attackPoint.localPosition = player.airForwardPos;
        player.currentHitboxSize = player.airForwardSize;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.IsGrounded()) stateMachine.ChangeState(player.groundedState);
    }

    public override void AnimationFinishTrigger() => stateMachine.ChangeState(player.airState);
}