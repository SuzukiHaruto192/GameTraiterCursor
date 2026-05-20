using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("--- ÂM THANH SINH TỒN ---")]
    [Header("Attack 1 SFX")]
    public AudioClip hitSound1;

    [Header("Attack 2 SFX")]
    public AudioClip hitSound2;

    [Header("Attack 3 SFX")]
    public AudioClip hitSound3;

    [Header("Air Up SFX")]
    public AudioClip upSound;

    [Header("Air Down SFX")]
    public AudioClip downSound;

    [Header("Air Forward SFX")]
    public AudioClip forwardSound;

    [Header("Jump SFX")]
    public AudioClip jumpSound;

    [Header("Dash SFX")]
    public AudioClip dashSound;

    [Header("Hurt SFX")]
    public AudioClip hurtSound;

    [Header("Die SFX")]
    public AudioClip dieSound;

    // --- HÀM PHỤ GIÚP RÚT GỌN CODE ---
    private void PlayAudio(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    // --- CÁC HÀM PHÁT ÂM THANH (Dùng cú pháp => cho ngắn gọn) ---
    public void PlaySoundATK1() => PlayAudio(hitSound1);
    public void PlaySoundATK2() => PlayAudio(hitSound2);
    public void PlaySoundATK3() => PlayAudio(hitSound3);

    public void PlaySoundUp() => PlayAudio(upSound);
    public void PlaySoundDown() => PlayAudio(downSound);
    public void PlaySoundForward() => PlayAudio(forwardSound);

    public void PlaySoundJump() => PlayAudio(jumpSound);
    public void PlaySoundDash() => PlayAudio(dashSound);
    public void PlaySoundHurt() => PlayAudio(hurtSound);
    public void PlaySoundDie() => PlayAudio(dieSound);
}