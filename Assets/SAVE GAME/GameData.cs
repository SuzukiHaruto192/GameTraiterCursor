using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Base stat")]
    public int baseAttack = 5;
    public int baseMaxHealth = 1000;
    public float baseJumpForce = 15;
    public float baseX = -6.48f;
    public float baseY = -6.08f;
    public string baseSceneName = "err";

    [Header("Player stat")]
    public int attack;
    public int maxHealth;
    public float jumpForce;

    [Header("Player position")]
    public float x;
    public float y;
    public string sceneName;

    [Header("Currency")]
    public int crystals;

    public void ClearData()
    {
        attack = baseAttack;
        maxHealth = baseMaxHealth;
        jumpForce = baseJumpForce;
        x = baseX; y = baseY;
        sceneName = baseSceneName;
        crystals = 3;
    }
}
