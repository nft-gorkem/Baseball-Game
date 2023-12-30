using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;
public class Ball : MonoBehaviour
{
    
    public Transform strikerPoint, baseGuy1, baseGuy2, baseGuy3, baseGuy4;
    public TextMeshProUGUI winnerText;
    bool catcher = false;
    public GameObject ballCatcher, GameScore, Striker;
    public GameObject StrikerCh, CatcherCh, ThrowerCh; // Reference to your striker object
    private Animator strikerAnimator, catcherAnimator, throwerAnimator; // Animator component of the striker
    private bool strikerFinished = false;
    private bool ballFinished = false, check = false;
    // Track the number of rounds played
    public int maxRounds = 9; // Maximum number of rounds
    public float strikerSpeed = 10f; // Striker's speed, adjust as needed
    public float strikerangularSpeed = 50000f;
    public float strikeraccelaration = 2000f;
    
    void Start() 
    {
        strikerAnimator = StrikerCh.GetComponent<Animator>();
        catcherAnimator = CatcherCh.GetComponent<Animator>();
        throwerAnimator = ThrowerCh.GetComponent<Animator>();
        // Initialize the game
        if(PlayerPrefs.GetInt("roundCount", 0) != 0)
        {
            StartRound();
        }
        else
        {
            Debug.Log("Press \"Space\" to start the game!");
        }
    }
    void FirstThrow()
    {
        transform.DOMove(strikerPoint.position, 2).OnComplete(BallHit);
    }

    void StartRound()
    {
        if (PlayerPrefs.GetInt("roundCount", 0)  < maxRounds)
        {
            int x = PlayerPrefs.GetInt("roundCount", 0);
            x++;
            PlayerPrefs.SetInt("roundCount", x);
            // Thrower Animation Triggers
            throwerAnimator.SetTrigger("startPitching");
            StartCoroutine(DelayedBallThrow());
            // Increase Striker's speed
            Striker.GetComponent<NavMeshAgent>().speed = strikerSpeed;
            Striker.GetComponent<NavMeshAgent>().angularSpeed = strikerangularSpeed;
            Striker.GetComponent<NavMeshAgent>().acceleration = strikeraccelaration;
            // Start the first throw
        }
        else
        {
            // Game finished after 9 rounds
            Debug.Log("Game finished");
            ShowWinner();
            print("sdadas");
            // Perform any end-of-game logic here
        }
    }

    IEnumerator DelayedBallThrow()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(2f);

        // Then perform the ball throw
        FirstThrow();
    }

    void ShowWinner()
    {
        int blueScore = PlayerPrefs.GetInt("BlueScore", 0);
        int redScore = PlayerPrefs.GetInt("RedScore", 0);

        if (blueScore > redScore)
        {
            winnerText.text = "BLUE TEAM WINS";
            winnerText.color = Color.blue; // Set text color to blue
        }
        else if (redScore > blueScore)
        {
            winnerText.text = "RED TEAM WINS";
            winnerText.color = Color.red; // Set text color to red
        }
        else
        {
            winnerText.text = "IT'S A TIE!";
            winnerText.color = Color.white; // Default color for a tie
        }

        // Optionally, make the winner text visible if it's not already
        winnerText.gameObject.SetActive(true);
    }
    void BallHit()
    {
    transform.parent = null;
    GetComponent<Rigidbody>().useGravity = true;
    GetComponent<Rigidbody>().isKinematic = false;

    // Generate a random forward direction within map limits
    Vector3 randomForwardDirection = GenerateRandomForwardDirection();

    // Apply force in the random forward direction
    GetComponent<Rigidbody>().AddForce(randomForwardDirection * 750);
    StartCoroutine(StrikertoTheBases());
    BallCatcher();
    IEnumerator StrikertoTheBases()
{
    NavMeshAgent strikerAgent = Striker.GetComponent<NavMeshAgent>();

    // Move the Striker to each base sequentially
    yield return MoveToStrikerBase(strikerAgent, baseGuy1.position);
    yield return MoveToStrikerBase(strikerAgent, baseGuy2.position);
    yield return MoveToStrikerBase(strikerAgent, baseGuy3.position);
    yield return MoveToStrikerBase(strikerAgent, baseGuy4.position);

    strikerFinished = true;
    CheckFinish();
}

    

IEnumerator MoveToStrikerBase(NavMeshAgent agent, Vector3 basePosition)
{
    strikerAnimator.SetBool("isRunning", true);
    strikerAnimator.SetBool("isIdle", false);
    agent.destination = basePosition;

    // Wait until the striker is close enough to the base
    while (Vector3.Distance(agent.transform.position, basePosition) > 1.5f) // 0.5 can be adjusted based on how close you want the striker to get to the base
    {
        yield return null; // Wait for the next frame
    }
    // Set idle animation
        strikerAnimator.SetBool("isRunning", false);
        strikerAnimator.SetBool("isIdle", true);
    // Optionally, wait for a brief moment at each base
    yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
}

}

    Vector3 GenerateRandomForwardDirection()
{
    // Customize these values based on your map's limits and desired ball behavior
    float minForward = -0.8f; // Minimum forward force
    float maxForward = -1.2f; // Maximum forward force
    float maxSideways = -2f; // Maximum sideways force, to keep it generally forward

    float forwardForce = Random.Range(minForward, maxForward);
    float sidewaysForce = Random.Range(-0.5f, maxSideways);

    // Ensure the ball always moves forward and within map limits
    return new Vector3(sidewaysForce, 0.35f, forwardForce);
}

    void BallCatcher()
    {
        catcher = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball Catcher")
        {
            transform.parent = other.gameObject.transform;
            GetComponent<Rigidbody>().isKinematic = true;
            catcher = false;
            transform.DOLocalMoveY(0.3f, 0.25f);
            other.transform.DORotate(new Vector3(0, -75, 0), 1).OnComplete(BalltoTheBases);
        }
    }

    void BalltoTheBases()
    {
        transform.parent = null;
        transform.DOMove(baseGuy1.position, 1.5f);
        transform.DOMove(baseGuy2.position, 1.5f).SetDelay(1.5f);
        transform.DOMove(baseGuy3.position, 1.5f).SetDelay(3.0f);
        transform.DOMove(baseGuy4.position, 1.5f).SetDelay(4.5f);
        CheckFinish();
    }
    void CheckFinish()
    {
        if (strikerFinished && !ballFinished)
        {
            Debug.Log("Striker reached Base 4 first!");
            int x = PlayerPrefs.GetInt("BlueScore",0);
            x++;
            PlayerPrefs.SetInt("BlueScore", x);
            
            //GameScore.UpdateTheText();
            PlayerPrefs.SetString("SomeText", "text content");
            PlayerPrefs.SetFloat("DecimalNumber", 0.5f);
        }
        else if (ballFinished && !strikerFinished)
        {
            Debug.Log("Ball reached Base 4 first!");
            int y = PlayerPrefs.GetInt("RedScore", 0);
            y++;
            PlayerPrefs.SetInt("RedScore", y);
            //GameScore.UpdateTheText();
        }
        if (strikerFinished || ballFinished)
        {
            RestartScene();
        }
    }
    
    void RestartScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
    }
    void Update()
    {
        if (catcher == true)
        {
            ballCatcher.GetComponent<NavMeshAgent>().destination = transform.position;

            // Check if the catcher is moving
            if (ballCatcher.GetComponent<NavMeshAgent>().velocity != Vector3.zero)
            {
                // Set running animation
                catcherAnimator.SetBool("isRunning", true);
                catcherAnimator.SetBool("isIdle", false);
            }
            
        }
        else
        {
            // Set idle animation
            catcherAnimator.SetBool("isRunning", false);
            catcherAnimator.SetBool("isIdle", true);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetAllData();
            RestartScene();
        }
        if(Input.GetKeyDown(KeyCode.Space) && PlayerPrefs.GetInt("roundCount") == 0)
        {
            StartRound();
        }

        if (catcher == true)
        {
            ballCatcher.GetComponent<NavMeshAgent>().destination = transform.position;
        }
        if(transform.position == baseGuy4.position && check == false)
        {
            ballFinished = true;
            CheckFinish();
            check = true;
        }
        

    }

}
