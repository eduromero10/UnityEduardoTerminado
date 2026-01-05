using UnityEngine;
using UnityEngine.InputSystem; // Nuevo Input System
using TMPro;
using UnityEngine.SceneManagement; // para cambiar / reiniciar escenas

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 10f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 6f;          // fuerza del salto
    [SerializeField] private float jumpCooldown = 0.2f;     // evita doble tap instantáneo
    [SerializeField] private Transform groundCheck;         // arrastra aquí el hijo GroundCheck
    [SerializeField] private float groundRadius = 0.5f;     // radio de la sonda al suelo
    [SerializeField] private LayerMask groundMask;          // ⬅️ SOLO la capa Ground

    [Header("UI")]
    public TMP_Text countText;
    public TMP_Text winText;

    [Header("Escenas / Reinicio")]
    [SerializeField] private string nextSceneName = "";     // nombre de la siguiente escena (Build Settings)
    [SerializeField] private string pickupTag = "PickUp";   // tag de los coleccionables
    [SerializeField] private string enemyTag = "Enemy";     // tag del enemigo
    [SerializeField] private string finishTag = "Finish";   // tag de zona de meta (opcional)

    // --- Privados ---
    private Rigidbody rb;
    private bool canJump = true;
    private bool isGrounded;
    private bool gameFinished = false;

    private float movementX;
    private float movementY;

    private int count;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (groundCheck == null) groundCheck = transform;

        count = 0;
        SetCountText();

        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // --- Detección de suelo robusta ---
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (!isGrounded)
        {
            isGrounded = Physics.Raycast(
                groundCheck.position + Vector3.up * 0.05f,
                Vector3.down,
                0.12f,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }

        // Input System + fallback clásico
        bool spacePressed =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            || Input.GetKeyDown(KeyCode.Space);

        // Saltar
        if (spacePressed && isGrounded && canJump && !gameFinished)
        {
            Jump();
        }

        // Reiniciar nivel con tecla R (cuando quieras)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartLevel();
        }
    }

    private void FixedUpdate()
    {
        if (gameFinished) return;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Coleccionables
        if (other.CompareTag(pickupTag))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
        // Enemigo: reinicia el nivel
        else if (other.CompareTag(enemyTag))
        {
            RestartLevel();
        }
        // Meta / zona de fin de nivel
        else if (other.CompareTag(finishTag))
        {
            FinishGame();
        }
    }

    // Acción "Move" (Vector2) mapeada en tu InputActions + PlayerInput (Send Messages)
    private void OnMove(InputValue movementValue)
    {
        Vector2 v = movementValue.Get<Vector2>();
        movementX = v.x;
        movementY = v.y;
    }

    private void Jump()
    {
        canJump = false;

        // Normaliza salto (quita velocidad vertical previa)
        Vector3 v = rb.linearVelocity;   // ARREGLADO: antes usabas linearVelocity (no existe)
        v.y = 0f;
        rb.linearVelocity = v;

        // Impulso instantáneo hacia arriba
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        // Cooldown para evitar dobles taps en el mismo frame de suelo
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump() => canJump = true;

    private void SetCountText()
    {
        if (countText != null)
            countText.text = "Count: " + count;

        // Cuando llegues a X coleccionables, muestra Win
        if (winText != null && count >= 7 && !gameFinished)
        {
            winText.gameObject.SetActive(true);
            winText.text = "¡Has recogido todos!";
        }
    }

    private void FinishGame()
    {
        gameFinished = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "¡Nivel completado!";
        }

        // Cambia de escena si has puesto nombre en nextSceneName
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Invoke(nameof(LoadNextLevel), 2f); // pequeña pausa
        }
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
