using UnityEngine;

public class PlayerHurtState : PlayerState
{
    public PlayerHurtState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("Hurt"); // Gọi trigger Hurt
        player.canDash = false; // Khóa Dash
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Trong lúc bị thương, tốc độ trục X giảm dần để tạo cảm giác trượt do văng lùi
        player.SetVelocity(player.rb.linearVelocity.x * 0.9f, player.rb.linearVelocity.y);

        // isAnimationFinished là biến có sẵn trong PlayerState gốc của bạn
        if (isAnimationFinished)
        {
            if (player.IsGrounded())
                stateMachine.ChangeState(player.groundedState);
            else
                stateMachine.ChangeState(player.airState);
        }
    }
}