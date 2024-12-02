using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] TMP_InputField circleCountIF;
    [SerializeField] TMP_InputField circleRadiusIF;

    public void ReloadButtonClicked()
    {
        if (int.TryParse(circleCountIF.text, out int count))
        {
            if (count > 0)
            {
                PlayerPrefs.SetInt("circle", count);
            }
        }

        if (float.TryParse(circleRadiusIF.text, out float radius))
        {
            if (radius > 0)
            {
                PlayerPrefs.SetFloat("radius", radius);
            }
        }

        SceneManager.LoadScene(0);
    }
}
