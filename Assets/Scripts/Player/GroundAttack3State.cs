using UnityEngine;

public class GroundAttack3State : PlayerState
{
    public GroundAttack3State(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("Attack3"); // Gọi trigger 3
        player.attackPoint.localPosition = player.atk3Pos;
        player.currentHitboxSize = player.atk3Size;
        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, player.GetComponent<Rigidbody2D>().linearVelocity.y);
    }

    // Đòn 3 không có bộ nhớ combo vì nó là đòn cuối
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        // Chém xong đòn 3 thì quay về Idle
        stateMachine.ChangeState(player.groundedState);
    }
}