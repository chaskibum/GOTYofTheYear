using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fadeToBlack;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
    
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);
        while (fadeToBlack.color.a < 1)
        {
            fadeToBlack.color = new Color(0, 0, 0, fadeToBlack.color.a + Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
            
        SceneManager.LoadScene("MainMenu");
    }
}
