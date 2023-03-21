using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public int level;

    [SerializeField] Text textLevel;

    void Start()
    {
        if (level == 0) return;

        textLevel.text = "Level " + level.ToString();
    }

    public void OpenSence()
    {
        if (textLevel != null)
        {
            SceneManager.LoadScene("Level " + level.ToString());
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
