using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController sharedInstance;

    public Transform target;

    float? minX = null;
    float? minY = null;
    float? maxX = null;
    float? maxY = null;

    public float speed = 1f;

    GameManager gManager;

    Vector3 direction;

    public Light2D cameraLight;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        this.gManager = GameManager.sharedInstance;
        direction = SetDirection();
    }

    public void SetBounds(Transform xMin, Transform xMax, Transform yMin, Transform yMax)
    {
        minX = xMin.position.x + Camera.main.orthographicSize * Camera.main.aspect;
        maxX = xMax.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        minY = yMin.position.y + Camera.main.orthographicSize;
        maxY = yMax.position.y - Camera.main.orthographicSize;
    }

    // Update is called once per frame

    Vector3 SetDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        return new Vector2(randomX, randomY).normalized;
    }
    void Update()
    {
        if (this.gManager.gameState == GameState.MainMenu || this.gManager.gameState == GameState.Controls || this.gManager.gameState == GameState.Instructions)
        {

            if (!this.cameraLight.gameObject.activeSelf)
            {
                this.cameraLight.gameObject.SetActive(true);
            }

            transform.Translate(direction * speed * Time.deltaTime);

            if (this.transform.position.x <= minX || this.transform.position.x >= maxX || this.transform.position.y <= minY || this.transform.position.y >= maxY)
            {
                direction = SetDirection();
            }
        }
        else
        {
            if (this.cameraLight.gameObject.activeSelf)
            {
                this.cameraLight.gameObject.SetActive(false);
            }

            if (minX != null && maxX != null && minY != null && maxY != null)
            {
                transform.position = new Vector3(Mathf.Clamp(target.position.x, minX.Value, maxX.Value), Mathf.Clamp(target.transform.position.y, minY.Value, maxY.Value), transform.position.z);
            }
            else
            {
                transform.position = target.position;
            }
        }

        
    }
}
