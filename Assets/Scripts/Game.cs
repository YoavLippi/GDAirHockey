using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    [SerializeField] private int AIScore = 0, PlayerScore = 0;

    [SerializeField] private TextMeshProUGUI textDisplay, AIText, PlayerText;

    [SerializeField] private GameObject Portal1, Portal2, puck;

    [SerializeField] private GameObject button;

    [SerializeField] private TextMeshProUGUI buttonText;

    private bool active;


    // Update is called once per frame
    void Update()
    {
    }

    void PlayerScored()
    {
        TempDisplay("Player scored!");
        //Invoke(nameof(TempDisplay), 1);
        PlayerScore++;
        PlayerText.text = "" + PlayerScore;
        if (PlayerScore == 5)
        {
            textDisplay.text = "Player Wins!";
            resetState();
            PlayerScore = 0;
            AIScore = 0;
            AIText.text = "" + 0;
            PlayerText.text = "" + 0;
        }
    }

    void AIScored()
    {
        TempDisplay("AI scored!");
        //Invoke(nameof(TempDisplay), 1);
        AIScore++;
        AIText.text = "" + AIScore;
        if (AIScore == 5)
        {
            textDisplay.text = "AI Wins!";
            resetState();
            AIScore = 0;
            PlayerScore = 0;
            PlayerText.text = "" + 0;
            AIText.text = "" + 0;
        }
    }

    private void resetState()
    {
        BroadcastMessage("Begin");
        StopCoroutine(nameof(SpawnTimer));
        button.SetActive(true);
        buttonText.text = "Restart?";
        Portal1.SetActive(false);
        Portal2.SetActive(false);
        puck.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        puck.gameObject.transform.position = new Vector3(0, 0, 0);
        puck.gameObject.SetActive(true);
    }

    void DoubleHit(string hitter)
    {
        if (hitter.Equals("AI Paddle"))
        {
            //to a minimum of zero
            if (AIScore != 0)
            {
                AIScore--;
                AIText.text = "" + AIScore;
            }
        }
        else
        {
            if (PlayerScore != 0)
            {
                PlayerScore--;
                PlayerText.text = "" + PlayerScore;
            }
        }
    }

    private void TempDisplay(string displayMessage)
    {
        textDisplay.text = displayMessage;
    }

    private void realStart()
    {
        StartCoroutine(nameof(RealStart));
        button.SetActive(false);
    }

    private IEnumerator RealStart()
    {
        for (int i = 3; i > 0; i--)
        {
            textDisplay.text = "" + i;
            yield return new WaitForSeconds(1f);
        }

        active = true;
        BroadcastMessage("Begin");
        StartCoroutine(nameof(SpawnTimer));
        textDisplay.text = "";
    }

    private IEnumerator SpawnTimer()
    {
        while (active)
        {
            yield return new WaitForSeconds(0.2f);
            int probability = Random.Range(1, 101);
            if (probability <= 5)
            {
                PlacePortals();
                Portal1.SetActive(true);
                Portal2.SetActive(true);
                Portal1.GetComponent<CapsuleCollider2D>().enabled = true;
                Portal2.GetComponent<CapsuleCollider2D>().enabled = true;
                int waitTime = Random.Range(15, 21);
                yield return new WaitForSeconds((float)waitTime);
                //6.5, -5.25
                Portal1.SetActive(false);
                Portal2.SetActive(false);
            }
        }
    }

    private void PlacePortals()
    {
        float[] YArr = new[] { 4.3f, -4.35f };
        float minX = -4.33f, maxX = 4.55f;

        float xPos1 = Random.Range(minX, maxX), xPos2 = 0;
        int yPos1Int = (int)Random.Range(0, 2);
        int yPos2Int = (int)Random.Range(0, 2);

        bool valid = false;
        while (!valid)
        {
            xPos2 = Random.Range(minX, maxX);
            if (!(xPos2 <= xPos1 + 1.4f && xPos2 >= xPos1 - 1.4f) || yPos1Int != yPos2Int)
            {
                valid = true;
            }
        }

        switch (yPos1Int)
        {
            case 0: //Upper side
                Portal1.transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
            case 1: //Lower side
                Portal1.transform.eulerAngles = new Vector3(0, 0, -90f);
                break;
        }

        switch (yPos2Int)
        {
            case 0: //Upper side
                Portal2.transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
            case 1: //Lower side
                Portal2.transform.eulerAngles = new Vector3(0, 0, -90f);
                break;
        }

        float yPos1 = YArr[yPos1Int];
        float yPos2 = YArr[yPos2Int];
        
        Portal1.transform.position = new Vector3(xPos1, yPos1);
        Portal2.transform.position = new Vector3(xPos2, yPos2);
    }
}