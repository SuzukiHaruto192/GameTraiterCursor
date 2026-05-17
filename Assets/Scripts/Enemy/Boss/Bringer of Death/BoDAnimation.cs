using UnityEngine;

public class BoDAnimation : MonoBehaviour
{
    private Animator anim;

    void Start() { anim = GetComponent<Animator>(); }

    public void TriggerTeleport()
    {
        if (anim != null) anim.SetTrigger("isTeleporting"); // Mở Animator tạo cái Trigger này nhé!
    }

    public void TriggerAttack() { if (anim != null) anim.SetTrigger("isAttacking"); }
    public void TriggerCast() { if (anim != null) anim.SetTrigger("isCasting"); }
    public void TriggerHurt() { if (anim != null) anim.SetTrigger("isHurting"); }
    public void TriggerDeath() { if (anim != null) anim.SetTrigger("isDying"); }
}