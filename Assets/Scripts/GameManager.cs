using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public GameObject enemies;
    public GameObject terrainManager;
    public GameObject cameraTop;
    public GameObject cameraFPV;
    public GameObject Bullets;
    public GameObject mainCamera;

    public AudioListener _audioListener;

    public Text scoreText;
    public Text healthText;
    public Text livesText;
    public Text levelText;
    public Text highScoreText;
    public Text gameOverText;
    public Text gameOverHighText;
    public Text continueText;
    public Text HSMenuText;
    public Text newHighScoreText;
    public Text modeSelectedText;
    public Text gameModePlayText;
    public Text HSHardMenuText;

    public Camera cameraTopCamera;
    public Camera cameraFPVCamera;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject panelContinue;
    public GameObject panelControls;
    public GameObject panelOptions;

    ///////////////////////

    public int _health;
    public int _score;
    public int _level;
    public int _lives;
    public int _highScore;
    public int _highScoreHard;
    public bool _isAlive;
    public bool gameIsOver;
    public string gameMode;
    public int _ammoQuantity;

    ///////////////////////

    bool _isSwitchingState;

    public static GameManager Instance { get; private set; }

    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER }
    State _state;

    public void PlayClicked()
    {
        SwitchState(State.INIT);
    }

    public void PlayAgainClicked()
    {
        SwitchState(State.INIT);
    }

    public void MenuClicked()
    {
        SwitchState(State.MENU);
    }

    public void ControlsClicked()
    {
        panelMenu.SetActive(false);
        panelControls.SetActive(true);
    }

    public void OptionsClicked()
    {
        panelMenu.SetActive(false);
        panelOptions.SetActive(true);
        modeSelectedText.text = "Game Mode: " + gameMode.ToString();
    }
    public void NormalModeClicked()
    {
        gameMode = "normal";
        modeSelectedText.text = "Game Mode: " + gameMode.ToString();
    }
    public void HardModeClicked()
    {
        gameMode = "hard";
        modeSelectedText.text = "Game Mode: " + gameMode.ToString();
    }

    public void QuitClicked()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        MainCamera();
        _audioListener = cameraFPV.GetComponent<AudioListener>();        
        gameIsOver = false;
        gameMode = "normal";
        _ammoQuantity = 0;
        SwitchState(State.MENU);
    }

    public void SwitchState(State newState, float delay = 0)
    {
        StartCoroutine(SwitchDelay(newState, delay));
    }

    IEnumerator SwitchDelay(State newState, float delay)
    {
        _isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        _state = newState;
        BeginState(newState);
        _isSwitchingState = false;
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MENU:
                panelControls.SetActive(false);
                panelOptions.SetActive(false);
                //PlayerPrefs.SetInt("highscore", 0); //Reset the score
                _highScore = PlayerPrefs.GetInt("highscore");
                _highScoreHard = PlayerPrefs.GetInt("highScoreHard");
                HSMenuText.text = "Highest Normal: " + _highScore;
                HSHardMenuText.text = "Highest Hard: " + _highScoreHard;
                Cursor.visible = true;
                panelMenu.SetActive(true);
                panelContinue.SetActive(false);
                player.SetActive(false);
                break;

            case State.INIT:                
                gameIsOver = false;
                _isAlive = true;
                player.GetComponent<Player>().isAlive = _isAlive;
                SwitchState(State.LOADLEVEL);
                break;

            case State.PLAY:

                CameraFPV();
                if (gameMode == "normal")
                {
                    gameModePlayText.text = "Mode: " + gameMode.ToString();
                }
                else if (gameMode == "hard")
                {
                    gameModePlayText.text = "Ammo: " + _ammoQuantity;
                }
                break;

            case State.LEVELCOMPLETED:

                panelLevelCompleted.SetActive(true);
                SwitchState(State.LOADLEVEL, 2f);
                break;

            case State.LOADLEVEL:
                if (gameMode == "normal")
                {
                    highScoreText.text = "Highest Score: " + _highScore;
                }
                else if (gameMode == "hard")
                {
                    highScoreText.text = "Highest Score: " + _highScoreHard;
                }

                panelPlay.SetActive(true);
                player.SetActive(true);

                cameraFPV.GetComponent<CameraMotor>().transition = 0.00f;

                SwitchState(State.PLAY);

                break;

            case State.GAMEOVER:
                gameIsOver = true;
                _highScore = PlayerPrefs.GetInt("highscore");
                _highScoreHard = PlayerPrefs.GetInt("highScoreHard");
                Cursor.visible = true;
                if (gameMode == "normal")
                {
                    if (_score > _highScore)
                    {
                        newHighScoreText.text = ("Congratulations, you have set a new Highest Score!!!");
                        PlayerPrefs.SetInt("highscore", _score);
                    }
                    else if (_highScore > _score)
                    {
                        newHighScoreText.text = "";
                    }
                    gameOverHighText.text = "Highest Score: " + _highScore.ToString();
                }

                if (gameMode == "hard")
                {
                    if (_score > _highScoreHard)
                    {
                        newHighScoreText.text = ("Congratulations, you have set a new Highest Score!!!");
                        PlayerPrefs.SetInt("highScoreHard", _score);
                    }
                    else if (_highScoreHard > _score)
                    {
                        newHighScoreText.text = "";
                    }
                    gameOverHighText.text = "Highest Score: " + _highScoreHard.ToString();
                }

                gameOverText.text = "Your Score: " + _score.ToString();
                MainCamera();
                panelGameOver.SetActive(true);
                enemies.GetComponent<Enemies>().ResetAll();
                terrainManager.GetComponent<TerrainManager>().ResetAll();
                player.GetComponent<Player>().ResetAll();
                player.SetActive(false);

                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case State.MENU:
                break;
            case State.INIT:
                break;
            case State.PLAY:

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Cursor.visible = false;
                    _isAlive = true;
                    player.GetComponent<Player>().isAlive = _isAlive;
                    panelContinue.SetActive(false);
                    panelPlay.SetActive(true);
                    player.SetActive(true);
                    CameraFPV();

                    Time.timeScale = 1;
                }

                if (!_isAlive)
                {
                    if (_lives >= 1)
                    {
                        MainCamera();
                        Cursor.visible = true;
                        panelPlay.SetActive(false);
                        panelContinue.SetActive(true);
                        player.SetActive(false);
                        continueText.text = "Lifes Left: " + (_lives).ToString();
                        //pause
                        Time.timeScale = 0;
                    }
                    else //if(_lives == 0)
                    {                        
                        SwitchState(State.GAMEOVER);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    MainCamera();
                    Cursor.visible = true;
                    panelPlay.SetActive(false);
                    panelContinue.SetActive(true);
                    player.SetActive(false);
                    continueText.text = "Lifes Left: " + (_lives).ToString();
                    //pause
                    Time.timeScale = 0;
                }
                break;

            case State.LEVELCOMPLETED:
                break;

            case State.LOADLEVEL:

                break;
            case State.GAMEOVER:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SwitchState(State.MENU);
                }
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cameraFPVCamera.isActiveAndEnabled)
            {
                CameraTop();
            }
            else if (cameraTopCamera.isActiveAndEnabled)
            {
                CameraFPV();
            }
        }

        //// TEXT Updates
        if (panelPlay.activeInHierarchy)
        {
            scoreText.text = "Score: " + _score.ToString();
            livesText.text = "Lives: " + _lives.ToString();
            levelText.text = "Level: " + _level.ToString();
            healthText.text = "Health: " + _health.ToString();

            if (gameMode == "hard")
            {
                if (_ammoQuantity > 0)
                {
                    gameModePlayText.text = "Ammo: " + _ammoQuantity;
                }
                if (_ammoQuantity <= 0)
                {
                    gameModePlayText.text = "OUT OF AMMO";
                }
            }
        }

    }

    void EndState()
    {
        switch (_state)
        {
            case State.MENU:
                panelMenu.SetActive(false);
                Cursor.visible = false;
                break;

            case State.INIT:
                break;

            case State.PLAY:
                panelPlay.SetActive(false);
                panelContinue.SetActive(false);

                break;

            case State.LEVELCOMPLETED:

                break;

            case State.LOADLEVEL:
                break;

            case State.GAMEOVER:
                panelGameOver.SetActive(false);
                gameIsOver = false;
                break;
            default:
                break;
        }
    }

    public void MainCamera()
    {
        if (!cameraTop.activeSelf)
        {
            cameraTop.SetActive(true);
        }
        if (!cameraFPV.activeSelf)
        {
            cameraFPV.SetActive(true);
        }
        cameraFPVCamera.GetComponent<AudioListener>().enabled = false;
        cameraTopCamera.enabled = false;
        cameraFPVCamera.enabled = false;
        mainCamera.SetActive(true);
    }

    public void CameraFPV()
    {
        mainCamera.SetActive(false);
        cameraTopCamera.enabled = false;
        cameraFPVCamera.enabled = true;
        cameraFPVCamera.GetComponent<AudioListener>().enabled = true;
        _audioListener.transform.SetParent(gameObject.transform);
        _audioListener.transform.position = Vector3.zero;
        _audioListener.transform.rotation = Quaternion.identity;

    }
    public void CameraTop()
    {
        mainCamera.SetActive(false);
        cameraFPVCamera.enabled = false;
        cameraTopCamera.enabled = true;
        _audioListener.transform.SetParent(cameraTopCamera.transform);
        _audioListener.transform.position = Vector3.zero;
        _audioListener.transform.rotation = Quaternion.identity;
    }
}
