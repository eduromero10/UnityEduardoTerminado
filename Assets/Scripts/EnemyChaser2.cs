using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyChaserLevel2 : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;          // si quieres, arrastras el Player aquí
    [SerializeField] private string playerTag = "Player";

    [Header("Chase")]
    [SerializeField] private float moveForce = 18f;
    [SerializeField] private float maxSpeed = 6f;

    private Rigidbody rb;
    private PlayerController playerController;       

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null)
            {
                player = p.transform;
                playerController = p.GetComponent<PlayerController>();
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerController != null && playerController.IsGameFinished())
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        dir.Normalize();

        rb.AddForce(dir * moveForce, ForceMode.Acceleration);

        Vector3 v = rb.linearVelocity;
        Vector3 h = new Vector3(v.x, 0f, v.z);

        if (h.magnitude > maxSpeed)
        {
            h = h.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(h.x, v.y, h.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerController != null && playerController.IsGameFinished())
            return;

        if (collision.collider.CompareTag(playerTag))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
