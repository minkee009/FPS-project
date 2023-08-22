using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

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
        playerHit = GameObject.Find("Actor").GetComponent<HitableObj>();
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

        stateText.color = new Color(255, 255, 255, 0f);
        state = GameState.Start;
    }
}
