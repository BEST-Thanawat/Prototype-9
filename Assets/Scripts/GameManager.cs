using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using LevelManagement;
using LevelManagement.Menus;

public class GameManager : MonoBehaviour
{
    // reference to player
    private GameObject _player;

    // reference to goal effect
    private GoalEffect _goalEffect;

    // reference to player
    private Objective _objective;
    private float timer = 0, timerMax = 0;

    private bool _isGameOver;
    public bool IsGameOver { get { return _isGameOver; } }

    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    [SerializeField]
    private TransitionFader _endTransitionPrefab;
    [SerializeField]
    private float WaitTimeToNextScene = 0.5f;

    // initialize references
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        _player = GameObject.FindGameObjectWithTag("Player");//Object.FindObjectOfType<ActorController>();
        _objective = Object.FindObjectOfType<Objective>();
        _goalEffect = Object.FindObjectOfType<GoalEffect>();
    }
    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
    // end the level
    public void EndLevel()
    {
        if (_player != null && Waited(WaitTimeToNextScene))
        {
            // disable the player controls
            //MotionController thirdPersonControl = _player.GetComponent<MotionController>();
            //if (thirdPersonControl != null)
            //{
                //thirdPersonControl.enabled = false;
            //}

            // remove any existing motion on the player
            Rigidbody rbody = _player.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.velocity = Vector3.zero;
            }

            // force the player to a stand still
            //ActorController actor = _player.GetComponent<ActorController>();
            //actor.UseTransformPosition = true;
        }

        // check if we have set IsGameOver to true, only run this logic once
        if (!_isGameOver) //_goalEffect != null &&
        {
            _isGameOver = true;
            //_goalEffect.PlayEffect();
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator WinRoutine()
    {
        TransitionFader.PlayTransition(_endTransitionPrefab);
        float fadeDelay = (_endTransitionPrefab != null) ? _endTransitionPrefab.Delay + _endTransitionPrefab.FadeOnDuration : 0f;
        yield return new WaitForSeconds(fadeDelay);
        WinScreeen.Open();
    }

    private void Update()
    {
        if (_objective != null && _objective.IsComplete)
        {
            EndLevel();
        }
    }
    private bool Waited(float seconds)
    {
        timerMax = seconds;
        timer += Time.deltaTime;

        if (timer >= timerMax)
        {
            return true; //max reached - waited x - seconds
        }

        return false;
    }
}