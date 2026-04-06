using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.canDash = true;
        player.canAirAttack = true;
        player.canDoubleJump = true; // Sạc lại khi bám tường
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Liên tục sạc đầy thời gian Coyote và ghi nhớ hướng của bức tường
        player.wallCoyoteTimer = player.wallCoyoteTime;
        player.lastWallDirection = player.isFacingRight ? 1 : -1;

        // 1. Ép trục Y trượt xuống từ từ
        player.SetVelocity(0, -player.wallSlideSpeed);

        float xInput = Input.GetAxisRaw("Horizontal");

        // 2. Chạm đất -> Về trạng thái Đứng
        if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.groundedState);
            return;
        }

        // 3. Hết tường để bám, hoặc bấm phím ngược hướng -> Rơi tự do
        if (!player.IsTouchingWall() || (player.isFacingRight && xInput < 0) || (!player.isFacingRight && xInput > 0))
        {
            stateMachine.ChangeState(player.airState);
            return;
        }

        // 4. WALL JUMP: Bấm K để nảy ra
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.wallCoyoteTimer = 0; // [MỚI] Tiêu hao ngay Coyote Time để tránh bị nhảy đúp
            
            float jumpDirection = player.isFacingRight ? -1f : 1f;
            player.SetVelocity(player.speed * jumpDirection, player.jumpForce);
            stateMachine.ChangeState(player.airState);
            return;
        }
    }
}