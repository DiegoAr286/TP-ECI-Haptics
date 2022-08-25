using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    new Rigidbody2D rigidbody;

    public float moveSpeed; // Velocidad de movimiento
    public float rotateAmount; // Cantidad que rota con cada click
    private float rotate; // Variable que contiene el valor de la rotación
    private int score;

    private int lives = 3;

    private bool isInvisible = false; // true cuando sale de la visión de la cámara, false cuando vuelve a ser visto
    private Vector3 currentPosition;

    public GameObject winText; // Texto que aparece al ganar
    public GameObject[] hearts;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Desactiva el texto de victoria
        winText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)) // Rota izquierda o derecha según la posición del dedo sobre la pantalla
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(mousePos.x < 0)
            {
                rotate = rotateAmount;
            }
            else
            {
                rotate = -rotateAmount;
            }

            transform.Rotate(0, 0, rotate); // Da la rotación del jugador
        }
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = transform.up * moveSpeed; // Le da el movimiento al jugador
    }


    private void OnTriggerEnter2D(Collider2D col) // Función que se llama al interactuar el jugador con un objeto
    {
        if (col.gameObject.tag == "Food") // Toca comida
        {
            score++;

            if (score == 5) // Si el score es 5, gana
            {
                winText.SetActive(true);
                StartCoroutine(WaitToRestart(3f)); // Espera algunos segundos para reiniciar el juego
            }
            long[] pattern = { 0, 100, 100, 100 }; // Vibra por 100 ms, pausa por 100 ms y vuelve a vibrar por 100 ms
            Vibrator.Vibrate(pattern, -1); // Vibra una sola vez en ese patrón
        }
        else if (col.gameObject.tag == "Poison")
        {
            if (lives > 1)
            {
                lives--;
                hearts[lives].SetActive(false);
            }
            else
            {
                hearts[0].SetActive(false);
                SceneManager.LoadSceneAsync("FoodEaterVibration");
            }
            Vibrator.Vibrate(500);
        }
        Destroy(col.gameObject);
    }
    IEnumerator WaitToRestart(float t) // Espera t segundos para reiniciar el juego
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadSceneAsync("FoodEaterVibration");
    }

    private void OnBecameInvisible() // Función que se llama cuando el jugador sale de la vista de la cámara
    {
        isInvisible = true;
        currentPosition = transform.position;
        GetComponent<TrailRenderer>().emitting = false; // Desactiva la estela producida por el jugador
        transform.position = -currentPosition; // "Teletransporta" al jugador al otro lado de la pantalla
    }

    private void OnBecameVisible() // Función que se llama cuando el jugador está a la vista de la cámara
    {
        if (isInvisible)
        {
            GetComponent<TrailRenderer>().emitting = true; // Reactiva la estela producida por el jugador
            isInvisible = false;
        }
    }
}