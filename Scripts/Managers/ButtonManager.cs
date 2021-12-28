using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static int btnResult;
    public GameManager gameManager;
    public Button btnAi;
    public Button btnOnline;
    public Button btnOffline;

    private void Start()
    {
        btnResult = -1;
    }

    public void checkBtnClick()
    {
        if (gameObject.name == "ButtonAi")
        {
            btnResult = 1;
            
            btnAi.gameObject.SetActive(false);
            btnOnline.gameObject.SetActive(false);
            btnOffline.gameObject.SetActive(false);

            gameManager.play(btnResult);
        }
        else if (gameObject.name == "ButtonOnline")
        {
            btnResult = 2;
            
            btnAi.gameObject.SetActive(false);
            btnOnline.gameObject.SetActive(false);
            btnOffline.gameObject.SetActive(false);

            gameManager.play(btnResult);
        }
        else if (gameObject.name == "ButtonOffline")
        {
            btnResult = 3;

            btnAi.gameObject.SetActive(false);
            btnOnline.gameObject.SetActive(false);
            btnOffline.gameObject.SetActive(false);

            gameManager.play(btnResult);
        }
        else
        {
            Debug.Log("ELSE");
        }
    }
}
