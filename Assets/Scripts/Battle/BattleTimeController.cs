using UnityEngine;

public class BattleTimeController : MonoBehaviour
{
    public float CurrentSpeed { get; private set; } = 1f;
    public bool IsPaused { get; private set; }

    void Awake()
    {
        CurrentSpeed = GameConfig.DefaultBattleSpeed;
    }

    public void Play()
    {
        IsPaused = false;
        Time.timeScale = CurrentSpeed;
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void SetSpeed(float multiplier)
    {
        CurrentSpeed = Mathf.Clamp(multiplier, 0.1f, GameConfig.MaxBattleSpeed);
        if (!IsPaused)
            Time.timeScale = CurrentSpeed;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : CurrentSpeed;
    }
}
