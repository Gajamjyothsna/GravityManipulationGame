using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public float timeLimit = 120f;
    public TextMeshProUGUI timerText;
    public Transform player;
    public LayerMask groundLayer;

    void Update()
    {
        UpdateTimer();
        CheckGameOverConditions();
    }

    private void UpdateTimer()
    {
        timeLimit -= Time.deltaTime;
        timerText.text = "Time Left: " + Mathf.Max(0, Mathf.RoundToInt(timeLimit)).ToString();

        if (timeLimit <= 0)
        {
            GameOver();
        }
    }

    private void CheckGameOverConditions()
    {
        if (!IsGrounded())
        {
            GameOver();
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(player.position, 0.1f, groundLayer);
    }

    private void GameOver()
    {
        // Implement game over logic
        Debug.Log("Game Over!");
    }
}
