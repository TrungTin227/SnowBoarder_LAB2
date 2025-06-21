using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 3f; // Tăng thời gian để hiển thị score
    [SerializeField] private ParticleSystem finishEffect;
    [SerializeField] private AudioClip finishSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            finishEffect.Play();
            GetComponent<AudioSource>().PlayOneShot(finishSound);

            // Hiển thị tổng điểm
            if (ScoreManager.Instance != null)
            {
                int finalScore = ScoreManager.Instance.GetTotalScore();
                Debug.Log($"Level Complete! Final Score: {finalScore:N0}");
            }

            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}