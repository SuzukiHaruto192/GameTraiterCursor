using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    // Gọi hàm này lần đầu tiên khi bắt đầu game
    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // Đổi sang một trạng thái mới (ví dụ: từ Chém 1 sang Chém 2)
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}