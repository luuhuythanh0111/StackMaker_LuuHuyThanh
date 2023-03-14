using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Text scoreText;

    public void SetScore(int score)
    {
        scoreText.text = "Score : " + score.ToString();
    }

}
