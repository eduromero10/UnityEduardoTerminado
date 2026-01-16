using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float acceleration = 35f;
    [SerializeField] float maxSpeed = 8.5f;
    [SerializeField] float airControl = 0.6f;

    [Header("Salto")]
    [SerializeField] float jumpImpulse = 7.5f;
    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBufferTime = 0.12f;
    [SerializeField] float jumpCutMultiplier = 0.5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.25f;
    [SerializeField] LayerMask groundMask;

    [Header("UI")]
    public TMP_Text countText;
    public GameObject winMessage;

    [Header("Fin de nivel")]
    [SerializeField] bool isLastLevel = false;        
    [SerializeField] bool restartToFirstLevel = false; 
    [SerializeField] bool finishOnAllPickups = false;  
    [SerializeField] GameObject finishPortal;          
    [SerializeField] string finishTag = "Finish";
    [SerializeField] string pickupTag = "PickUp";

    Rigidbody rb;
    bool finished;
    bool grounded;

    float moveX, moveY;
    int count, total;
    float coyote, buffer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (groundCheck == null) groundCheck = transform;

        total = GameObject.FindGameObjectsWithTag(pickupTag).Length;

        if (finishPortal != null) finishPortal.SetActive(false);
        if (winMessage != null) winMessage.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            Restart();

        if (finished) return;

        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        coyote = grounded ? coyoteTime : coyote - Time.deltaTime;

        bool jumpDown = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.Space);
        bool jumpUp   = (Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame) || Input.GetKeyUp(KeyCode.Space);

        buffer = jumpDown ? jumpBufferTime : buffer - Time.deltaTime;

        if (buffer > 0f && coyote > 0f)
        {
            Jump();
            buffer = 0f;
            coyote = 0f;
        }

        if (jumpUp && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier, rb.linearVelocity.z);
    }

    void FixedUpdate()
    {
        if (finished) return;

        float control = grounded ? 1f : airControl;

        rb.AddForce(new Vector3(moveX, 0f, moveY) * (acceleration * control), ForceMode.Acceleration);

        Vector3 v = rb.linearVelocity;
        Vector3 h = new Vector3(v.x, 0f, v.z);
        if (h.magnitude > maxSpeed)
        {
            h = h.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(h.x, v.y, h.z);
        }
    }

    void OnMove(InputValue value)
    {
        if (finished) return;
        Vector2 v = value.Get<Vector2>();
        moveX = v.x;
        moveY = v.y;
    }

    void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        if (other.CompareTag(pickupTag))
        {
            other.gameObject.SetActive(false);
            count++;
            UpdateUI();

            if (finishOnAllPickups && count >= total)
                Finish();

            return;
        }

        if (!finishOnAllPickups && other.CompareTag(finishTag) && count >= total)
            Finish();
    }

    void UpdateUI()
    {
        if (countText != null)
            countText.text = $"Count: {count} / {total}";

        if (!finishOnAllPickups && finishPortal != null && count >= total)
            finishPortal.SetActive(true);
    }

    void Jump()
    {
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;
        rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
    }

    void Finish()
    {
        finished = true;

        moveX = moveY = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (winMessage != null) winMessage.SetActive(true);
        else Debug.LogWarning("winMessage no asignado en esta escena.");

        if (!isLastLevel && !finishOnAllPickups)
            Invoke(nameof(LoadNext), 2f);
    }

    void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Restart()
    {
        SceneManager.LoadScene(restartToFirstLevel ? 0 : SceneManager.GetActiveScene().buildIndex);
    }
    public bool IsGameFinished()
{
    return finished;
}

}
