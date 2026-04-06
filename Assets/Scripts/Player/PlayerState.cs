using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    protected float startTime; // Dùng để đo thời gian State đã chạy
    protected string animBoolName; // Tên của parameter trong Animator

    // BỔ SUNG BIẾN QUAN TRỌNG
    protected bool isAnimationFinished;

    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        // Ghi nhận thời điểm bắt đầu vào state
        startTime = Time.time;

        // Reset biến này mỗi khi bắt đầu một hành động mới
        isAnimationFinished = false;

        // Bật animation tương ứng (chỉ bật nếu có tên để tránh lỗi với Trigger rỗng "")
        if (animBoolName != "")
        {
            player.anim.SetBool(animBoolName, true);
        }
    }

    public virtual void LogicUpdate()
    {
        // Hàm này sẽ chạy mỗi frame (giống Update)
    }

    public virtual void Exit()
    {
        // Tắt animation khi thoát state
        if (animBoolName != "")
        {
            player.anim.SetBool(animBoolName, false);
        }
    }

    public virtual void AnimationFinishTrigger()
    {
        // Hàm này sẽ được gọi từ Animation Event để báo hiệu đã diễn xong
        isAnimationFinished = true;
    }
}