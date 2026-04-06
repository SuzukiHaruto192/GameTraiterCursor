using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Trừ dần thời gian ân hạn (Coyote Time)
        if (player.wallCoyoteTimer > 0)
        {
            player.wallCoyoteTimer -= Time.deltaTime;
        }

        // KÍCH HOẠT WALL JUMP COYOTE (Nhảy vớt)
        if (Input.GetKeyDown(KeyCode.K) && player.wallCoyoteTimer > 0)
        {
            // Nảy ngược lại với hướng bức tường mà ta vừa rời khỏi
            float jumpDirection = -player.lastWallDirection;

            player.SetVelocity(player.speed * jumpDirection, player.jumpForce);
            player.wallCoyoteTimer = 0; // Xóa thời gian để không nhảy đúp được nữa
            return;
        }
        float xInput = Input.GetAxisRaw("Horizontal");

        // DOUBLE JUMP (Nhảy đúp giữa không trung)
        // Nếu bấm K, và CÒN lượt nhảy đúp, VÀ đã hết thời gian Coyote (để không bị trùng phím)
        if (Input.GetKeyDown(KeyCode.K) && player.canDoubleJump && player.wallCoyoteTimer <= 0)
        {
            player.canDoubleJump = false; // Tiêu hao lượt nhảy
            player.canAirAttack = true;

            // Xóa sạch đà rơi hiện tại và ép lực nảy mới
            player.SetVelocity(player.rb.linearVelocity.x, player.jumpForce);

            // Gọi lại Trigger nhảy để Animator phát lại dáng nhảy từ đầu
            // Lưu ý: Thay "Jump" bằng tên Parameter Trigger nhảy của bạn nếu bạn đặt khác
            player.anim.SetTrigger("Jump");
            return;
        }

        // 1. KIỂM TRA BÁM TƯỜNG TRƯỚC TIÊN (Mới thêm)
        if (player.IsTouchingWall() && player.rb.linearVelocity.y < 0)
        {
            if ((player.isFacingRight && xInput > 0) || (!player.isFacingRight && xInput < 0))
            {
                stateMachine.ChangeState(player.wallSlideState);
                return;
            }
        }

        // 2. Bấm L để Dash
        if (Input.GetKeyDown(KeyCode.L) && player.canDash)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

        // 3. Cho phép di chuyển nhẹ trên không
        player.SetVelocity(xInput * player.speed, player.rb.linearVelocity.y);
        player.anim.SetFloat("Speed", Mathf.Abs(xInput));

        // 4. Trọng lực biến thiên
        if (player.rb.linearVelocity.y < 0)
        {
            player.rb.gravityScale = player.fallMultiplier;
        }
        else if (player.rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.K))
        {
            player.rb.gravityScale = player.lowJumpMultiplier;
        }
        else
        {
            player.rb.gravityScale = 1f;
        }

        // 5. Tấn công trên không đa hướng (Đã thêm điều kiện canAirAttack)
        if (Input.GetKeyDown(KeyCode.J) && player.canAirAttack)
        {
            player.canAirAttack = false; // Tiêu hao ngay lập tức quyền chém

            float yInput = Input.GetAxisRaw("Vertical");
            if (yInput > 0) stateMachine.ChangeState(player.airAttackUpState);
            else if (yInput < 0) stateMachine.ChangeState(player.airAttackDownState);
            else stateMachine.ChangeState(player.airAttackForwardState);
            return;
        }

        // 6. Xử lý chạm đất
        if (player.IsGrounded() && player.rb.linearVelocity.y <= 0.1f)
        {
            player.rb.gravityScale = 1f;
            stateMachine.ChangeState(player.groundedState);
        }
    }
}