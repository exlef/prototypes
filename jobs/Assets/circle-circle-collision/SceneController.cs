using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    public void ReloadButtonClicked()
    {
        int i = -1;
        if (int.TryParse(inputField.text, out i))
        {
            if (i > 0)
            {
                PlayerPrefs.SetInt("circle", i);
            }
        }

        SceneManager.LoadScene(0);
    }
}
