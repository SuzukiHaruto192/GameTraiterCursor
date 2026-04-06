using UnityEngine;

public class GroundAttack2State : PlayerState
{
    private bool shouldCombo;

    public GroundAttack2State(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("Attack2"); // Gọi trigger 2\
        player.attackPoint.localPosition = player.atk2Pos;
        player.currentHitboxSize = player.atk2Size;
        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, player.GetComponent<Rigidbody2D>().linearVelocity.y);
        shouldCombo = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Input.GetKeyDown(KeyCode.J)) shouldCombo = true;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (shouldCombo)
            stateMachine.ChangeState(player.attack3State); // Nối sang đòn 3
        else
            stateMachine.ChangeState(player.groundedState);
    }
}