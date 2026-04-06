using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.canDash = true;
        player.canAirAttack = true;
        player.canDoubleJump = true; // Sạc lại khi chạm đất
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. Logic Di chuyển trái phải
        float xInput = Input.GetAxisRaw("Horizontal");
        player.SetVelocity(xInput * player.speed, player.rb.linearVelocity.y);

        // 2. Cập nhật Animator cho Run/Idle
        float speedAbs = Mathf.Abs(xInput);
        player.anim.SetFloat("Speed", speedAbs);
        player.anim.SetInteger("AnimState", speedAbs > 0.1f ? 1 : 0);

        // 3. Logic Đỡ đòn (Giữ S)
        if (Input.GetAxis("Vertical") < 0 && xInput == 0)
        {
            player.isInvincible = true;
            player.SetVelocity(0, 0);
        }
        else player.isInvincible = false;

        // ===============================================
        // 4. [CẬP NHẬT] XỬ LÝ NHẢY HOẶC LỌT XUỐNG BỆ GỖ
        // ===============================================
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Nếu đang đè phím XUỐNG (S hoặc Mũi tên xuống)
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                player.StartCoroutine(player.DropThroughOneWayPlatform());
                player.SetVelocity(player.rb.linearVelocity.x, -5f);
                stateMachine.ChangeState(player.airState); // Rớt xuống thì tính là rơi tự do
                return;
            }
            // Nếu KHÔNG đè phím xuống (nhảy bình thường)
            else
            {
                player.SetVelocity(player.rb.linearVelocity.x, player.jumpForce);
                player.anim.SetTrigger("Jump");
                stateMachine.ChangeState(player.airState);
                return;
            }
        }

        // 5. Bấm J -> Chuyển sang State Tấn công 1
        if (Input.GetKeyDown(KeyCode.J))
        {
            stateMachine.ChangeState(player.attack1State);
            return;
        }

        // 6. Bấm L để Dash
        if (Input.GetKeyDown(KeyCode.L) && player.canDash)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

        // 7. Tự động rơi nếu bước khỏi rìa đất (Fall)
        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}