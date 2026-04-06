using UnityEngine;

public class GroundAttack1State : PlayerState
{
    private bool shouldCombo; // Biến ghi nhớ lệnh bấm phím

    public GroundAttack1State(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetTrigger("Attack1");
        player.attackPoint.localPosition = player.atk1Pos;
        player.currentHitboxSize = player.atk1Size;
        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, player.GetComponent<Rigidbody2D>().linearVelocity.y);
        shouldCombo = false; // Reset bộ nhớ mỗi lần vung kiếm
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Nếu người chơi bấm J trong lúc đang chém nhát 1, ghi nhớ lại!
        if (Input.GetKeyDown(KeyCode.J))
        {
            shouldCombo = true;
        }
    }

    // Khi Animation Event báo "Đã chém xong"
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        if (shouldCombo)
        {
            // Có bấm phím -> Nối sang nhát 2
            stateMachine.ChangeState(player.attack2State);
        }
        else
        {
            // Không bấm phím -> Thu kiếm, về tư thế chờ
            stateMachine.ChangeState(player.groundedState);
        }
    }
}