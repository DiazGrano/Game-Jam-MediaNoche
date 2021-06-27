using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    Playing,
    GameOver,
    Victory,
    MainMenu,
    Controls,
    Instructions
}

public enum PlayerAnimations
{
    Idle,
    Walking,
    Jumping,
    Falling, 
    Death
}

public enum PlayerState
{
    Alive,
    Death
}


public enum CollectableType
{
    Live
}

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;

    public GameState gameState;

    public int maxTime = 60;
    public int maxLives = 3;
    public float torchLightIntensity = 1f;

    public GameObject mainMenuBackground;
    private GameObject auxBackground;
    public List<GameObject> gameLevels = new List<GameObject>();
    public GameObject currentLevel;

    public Canvas mainMenuCanvas;
    public Canvas gameOverCanvas;
    public Canvas victoryCanvas;
    public Canvas gameUICanvas;
    public Canvas controlsCanvas;
    public Canvas instructionsCanvas;

    public TextMeshProUGUI mainMenuRecordText;
    public TextMeshProUGUI victoryRecordText;

    public GameObject livesContainer;
    public GameObject livePrefab;

    public TextMeshProUGUI timeText;

    public Text fpsText;
    public Text averagefpsText;

    private int remainingTimeSeconds;
    private float remainingTimeMilliseconds;


    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //StartCoroutine(FPS());

        SetGameState(GameState.MainMenu);
        //SetGame();
    }


    IEnumerator FPS()
    {
        int fps = 0;
        int averageFPS = 0;
        int counter = 1;
        while (true)
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
            this.fpsText.text = fps.ToString();


            averageFPS += fps;
            this.averagefpsText.text = (averageFPS / counter).ToString();

            counter++;
            yield return new WaitForSeconds(0.2f);

        }
    }

    public void LivesUI(int cantidad)
    {
        for (int i = 0; i < Mathf.Abs(cantidad); i++)
        {
            if (cantidad < 0)
            {
                if (this.livesContainer.transform.childCount > i)
                {

                    for (int j = 0; j < this.livesContainer.transform.childCount; j++)
                    {
                        if (!this.livesContainer.transform.GetChild(0).gameObject.activeSelf)
                        {
                            this.livesContainer.transform.GetChild(0).SetAsLastSibling();
                        }
                        else
                        {
                            this.livesContainer.transform.GetChild(0).gameObject.SetActive(false);
                            this.livesContainer.transform.GetChild(0).SetAsLastSibling();
                            break;
                        }
                    }


                }
                else
                {
                    break;
                }
            }
            else
            {
                if (this.livesContainer.transform.childCount > i)
                {
                    for (int j = 0; j  < this.livesContainer.transform.childCount; j ++)
                    {
                        if (this.livesContainer.transform.GetChild(0).gameObject.activeSelf)
                        {
                            this.livesContainer.transform.GetChild(0).SetAsLastSibling();
                        }
                        else
                        {
                            this.livesContainer.transform.GetChild(0).gameObject.SetActive(true);
                            break;
                        }
                    }
                    
                }
                else
                {
                    break;
                }
            }
        }
    }

    public Vector2 GetRecord()
    {
        return new Vector2(PlayerPrefs.GetFloat("record_seconds", 0f), PlayerPrefs.GetFloat("record_milliseconds", 0f));
    }

    public void SetRecord(float seconds, float milliseconds)
    {
        if (seconds < GetRecord().x)
        {
            return;
        }
        else if (seconds == GetRecord().x)
        {
            if (milliseconds < GetRecord().y)
            {
                return;
            }
        }
        PlayerPrefs.SetFloat("record_seconds", seconds);
        PlayerPrefs.SetFloat("record_milliseconds", milliseconds);
        Debug.Log("Record");

    }

    private void SetChildrenState(GameObject parent, bool enabled = true)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(enabled);
        }
    }


    private void SetGameState(GameState state)
    {
        StopAllCoroutines();
        this.gameState = state;
        switch (state)
        {
            case GameState.Playing:
                this.gameOverCanvas.enabled = false;
                this.mainMenuCanvas.enabled = false;
                this.victoryCanvas.enabled = false;
                this.controlsCanvas.enabled = false;
                this.gameUICanvas.enabled = true;
                this.instructionsCanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, false);
                SetChildrenState(this.gameOverCanvas.gameObject, false);
                SetChildrenState(this.mainMenuCanvas.gameObject, false);
                SetChildrenState(this.victoryCanvas.gameObject, false);
                //this.gameUICanvas.enabled = true;

                SetGame();
                StartCoroutine(Timer());

                break;
            case GameState.GameOver:
                this.gameOverCanvas.enabled = true;
                this.mainMenuCanvas.enabled = false;
                this.victoryCanvas.enabled = false;
                this.controlsCanvas.enabled = false;
                this.gameUICanvas.enabled = true;
                this.instructionsCanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, false);
                SetChildrenState(this.gameOverCanvas.gameObject, true);
                SetChildrenState(this.mainMenuCanvas.gameObject, false);
                SetChildrenState(this.victoryCanvas.gameObject, false);
                //this.gameUICanvas.enabled = true;
                break;
            case GameState.Victory:

                SetRecord(this.remainingTimeSeconds, this.remainingTimeMilliseconds);

                this.gameOverCanvas.enabled = false;
                this.mainMenuCanvas.enabled = false;
                this.victoryCanvas.enabled = true;
                this.controlsCanvas.enabled = false;
                this.gameUICanvas.enabled = true;
                this.instructionsCanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, false);
                SetChildrenState(this.gameOverCanvas.gameObject, false);
                SetChildrenState(this.mainMenuCanvas.gameObject, false);
                SetChildrenState(this.victoryCanvas.gameObject, true);


                this.victoryRecordText.text = FormatTime(GetRecord().x) + ":" + FormatTime(GetRecord().y);
                //this.gameUICanvas.enabled = true;
                break;
            case GameState.MainMenu:



                this.gameOverCanvas.enabled = false;
                this.mainMenuCanvas.enabled = true;
                this.victoryCanvas.enabled = false;
                this.controlsCanvas.enabled = false;
                this.instructionsCanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, false);
                SetChildrenState(this.gameOverCanvas.gameObject, false);
                SetChildrenState(this.mainMenuCanvas.gameObject, true);
                SetChildrenState(this.victoryCanvas.gameObject, false);
                this.gameUICanvas.enabled = false;

                Player.sharedInstance.sRenderer.enabled = false;
                Player.sharedInstance.torch.enabled = false;

                if (auxBackground == null)
                {
                    GameLevel auxGameLevel = Instantiate(this.mainMenuBackground).GetComponent<GameLevel>();

                    auxBackground = auxGameLevel.gameObject;

                    CameraController.sharedInstance.SetBounds(auxGameLevel.xAxisMin, auxGameLevel.xAxisMax, auxGameLevel.yAxisMin, auxGameLevel.yAxisMax);
                }
                if (this.currentLevel != null)
                {
                    Destroy(this.currentLevel);
                }

                this.mainMenuRecordText.text = FormatTime(GetRecord().x) + ":" + FormatTime(GetRecord().y);

                
                break;
            case GameState.Controls:
                this.gameOverCanvas.enabled = false;
                this.mainMenuCanvas.enabled = false;
                this.victoryCanvas.enabled = false;
                this.controlsCanvas.enabled = true;
                this.instructionsCanvas.enabled = false;
                this.gameUICanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, false);
                SetChildrenState(this.gameOverCanvas.gameObject, false);
                SetChildrenState(this.mainMenuCanvas.gameObject, false);
                SetChildrenState(this.victoryCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, true);
                break;

            case GameState.Instructions:
                this.gameOverCanvas.enabled = false;
                this.mainMenuCanvas.enabled = false;
                this.victoryCanvas.enabled = false;
                this.controlsCanvas.enabled = false;
                this.instructionsCanvas.enabled = true;
                this.gameUICanvas.enabled = false;
                SetChildrenState(this.instructionsCanvas.gameObject, true);
                SetChildrenState(this.gameOverCanvas.gameObject, false);
                SetChildrenState(this.mainMenuCanvas.gameObject, false);
                SetChildrenState(this.victoryCanvas.gameObject, false);
                SetChildrenState(this.controlsCanvas.gameObject, false);
                
                break;
        }
        
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }
    public void MainMenu()
    {
        SetGameState(GameState.MainMenu);
    }
    public void Victory()
    {
        SetGameState(GameState.Victory);
    }
    public void Play()
    {
        SetGameState(GameState.Playing);
    }

    public void Controls()
    {
        SetGameState(GameState.Controls);
    }

    public void Instructions()
    {
        SetGameState(GameState.Instructions);
    }

    public void ExitGame()
    {
        Application.Quit();
    }



    public bool Playing()
    {
        return this.gameState == GameState.Playing;
    }

    private void SetGame()
    {
        if (this.gameLevels.Count > 0)
        {
            if (this.currentLevel != null)
            {
                Destroy(this.currentLevel);
            }

            if (this.auxBackground != null)
            {
                Destroy(auxBackground);
            }



            int randomLevel = Random.Range(0, this.gameLevels.Count);
            this.currentLevel = Instantiate(this.gameLevels[randomLevel]);
            Player.sharedInstance.state = PlayerState.Alive;

            GameLevel auxGameLevel = this.currentLevel.GetComponent<GameLevel>();
            CameraController.sharedInstance.SetBounds(auxGameLevel.xAxisMin, auxGameLevel.xAxisMax, auxGameLevel.yAxisMin, auxGameLevel.yAxisMax);

            Player.sharedInstance.transform.position = auxGameLevel.startPoint.position;
            this.timeText.text = this.maxTime.ToString() + ":00";
            Player.sharedInstance.PlayerHealth(this.maxLives);

            Player.sharedInstance.startingTravelPosition = null;
            Player.sharedInstance.door = null;
            Player.sharedInstance.doorAnim = null;
            Player.sharedInstance.cCollider.enabled = true;
            Player.sharedInstance.rb.bodyType = RigidbodyType2D.Dynamic;
            Player.sharedInstance.rb.velocity = Vector2.zero;
            Player.sharedInstance.inDoor = false;
            Player.sharedInstance.sRenderer.enabled = true;
            Player.sharedInstance.torch.enabled = true;
            Player.sharedInstance.anim.SetTrigger("Normal");
            Player.sharedInstance.torch.intensity = this.torchLightIntensity;
            Player.sharedInstance.transform.localScale = new Vector3(1f, 1f, 1f);
            LivesUI((this.maxLives - this.livesContainer.transform.childCount));
        } 
    }

    IEnumerator Timer()
    {
        remainingTimeSeconds = this.maxTime;

        remainingTimeSeconds--;
        remainingTimeMilliseconds = 99f;


        float auxTorchLight = this.torchLightIntensity / (remainingTimeSeconds * 100);

        float currentTorchLightIntensity = this.torchLightIntensity;


        while (remainingTimeSeconds >= 0f)
        {
            yield return new WaitForFixedUpdate();
            remainingTimeMilliseconds -= Time.deltaTime * 100f;

            currentTorchLightIntensity -= auxTorchLight * Time.deltaTime * 100f;
            if (remainingTimeMilliseconds <= 0)
            {
                remainingTimeSeconds--;
                remainingTimeMilliseconds = 99f;
                
            }

            if (Player.sharedInstance.torch.isActiveAndEnabled)
            {
                Player.sharedInstance.torch.intensity = currentTorchLightIntensity;
            }
            string formatedSeconds = string.Format("{0:00}", remainingTimeSeconds);
            string formatedMiliseconds = string.Format("{0:00}", remainingTimeMilliseconds);
            this.timeText.text = formatedSeconds + ":" + formatedMiliseconds;
            //this.timeText.text = remainingTime.ToString() + ":" + (int)miliseconds;
        }
        this.timeText.text =   "00:00";
        GameOver();
        yield break;
    }

    public string FormatTime(float time)
    {
        return string.Format("{0:00}", time);
    }

}
