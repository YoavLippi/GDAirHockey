using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GoalBehaviour : MonoBehaviour
{
    [SerializeField] private string goalName;

    [SerializeField] private GameObject puck;

    [SerializeField] private AudioSource audioSource;
    

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        goalName = transform.name;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Puck"))
        {
            audioSource.Play(0);
            switch (goalName.ToUpper())
            {
                case "AIGOAL":
                    //Debug.Log("Player scored");
                    col.gameObject.SetActive(false);
                    SendMessageUpwards("PlayerScored");
                    StartCoroutine(nameof(ReturnBall));
                    break;
                case "PLAYERGOAL":
                    //Debug.Log("AI scored");
                    col.gameObject.SetActive(false);
                    SendMessageUpwards("AIScored"); 
                    StartCoroutine(nameof(ReturnBall));
                    break;
            }
        }
    }

    private IEnumerator ReturnBall()
    {
        if (active)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 3; i > 0; i--)
            {
                //Debug.Log(i);
                SendMessageUpwards("TempDisplay", $"{i}");
                yield return new WaitForSeconds(1f);
            }

            SendMessageUpwards("TempDisplay", "");
            puck.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            puck.gameObject.transform.position = new Vector3(0, 0, 0);
            puck.gameObject.SetActive(true);
        }
    }

    private void Begin()
    {
        active = !active;
        if (!active)
        {
            puck.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            puck.gameObject.transform.position = new Vector3(0, 0, 0);
            puck.gameObject.SetActive(true);
        }
    }
}