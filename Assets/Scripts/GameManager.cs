using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Ready,
        Start,
        GameOver
    }

    public GameState state = GameState.Ready;
    public TMP_Text stateText;
    public HitableObj playerHit;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        stateText.text = "Ready";
        stateText.color = new Color(255, 185, 0, 255);
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if(playerHit.Hp <= 0f)
        {
            state = GameState.GameOver;
            stateText.text = "Game Over";
            stateText.color = new Color(0.9222222f, 0.9716981f, 0.4904325f, 1f);
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);

        stateText.text = "Game Start";
        stateText.color = new Color(255, 255, 255, 255);

        yield return new WaitForSeconds(0.5f);

        playerHit = GameObject.Find("Actor").GetComponent<HitableObj>();
        stateText.color = new Color(255, 255, 255, 0f);
        state = GameState.Start;
    }

    public void SetPause()
    {
        Time.timeScale = 0.0f;
    }

    public void SetPlay()
    {
        Time.timeScale = 1.0f;
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SetPlay();

        state = GameState.Ready;
        stateText.text = "Ready";
        stateText.color = new Color(255, 185, 0, 255);
        StartCoroutine(StartGame());
       
    }
}
