using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashDirection;

    public PlayerDashState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();

        // Kích hoạt Animation Dash
        player.anim.SetTrigger("Dash");

        // 1. Khóa khả năng Dash cho đến khi chạm đất lại
        player.canDash = false;

        // 2. Xác định hướng lướt (dựa vào mặt nhân vật)
        dashDirection = player.isFacingRight ? 1f : -1f;

        // 3. Tắt trọng lực để bay thẳng
        player.rb.gravityScale = 0f;

        // 4. Bật hiệu ứng
        player.StartDashEffect();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Ép vận tốc lao về phía trước
        player.SetVelocity(player.dashForce * dashDirection, 0);

        // State Machine tự đếm thời gian! Nếu lớn hơn dashTime thì tự thoát
        if (Time.time >= startTime + player.dashTime)
        {
            if (player.IsGrounded())
                stateMachine.ChangeState(player.groundedState);
            else
                stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Trả lại trọng lực và tắt hiệu ứng
        player.rb.gravityScale = 1f;
        player.StopDashEffect();
        player.SetVelocity(0, 0); // Phanh lại ngay lập tức
    }
}