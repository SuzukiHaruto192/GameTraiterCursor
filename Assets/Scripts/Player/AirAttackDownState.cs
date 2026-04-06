using UnityEngine;

public class AirAttackDownState : PlayerState
{
    public AirAttackDownState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("AirAttackDown");
        player.attackPoint.localPosition = player.airDownPos;
        player.currentHitboxSize = player.airDownSize;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.IsGrounded()) stateMachine.ChangeState(player.groundedState);
    }

    public override void AnimationFinishTrigger() => stateMachine.ChangeState(player.airState);
}