using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("Dead"); // Gọi trigger Dead

        // Dừng mọi vận tốc di chuyển ngang, chỉ giữ lại trọng lực để xác rơi xuống
        player.SetVelocity(0, player.rb.linearVelocity.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // Không có lệnh ChangeState nào ở đây. Nhân vật sẽ kẹt ở Dead mãi mãi cho đến khi Load lại scene.
    }
}