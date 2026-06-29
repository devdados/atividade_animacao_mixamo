using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Transform myCamera;
    private Animator animator;

    private bool estaNoChao;
    [SerializeField] private Transform peDoPlayer;
    [SerializeField] private LayerMask colisaoLayer;

    private float forcaY;

    // 🔥 NOVO: velocidades
    [SerializeField] private float velocidadeAndar = 5f;
    [SerializeField] private float velocidadeCorrer = 10f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // =========================
        // 1. INPUT MOVIMENTO
        // =========================
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3(horizontal, 0, vertical);
        movimento = myCamera.TransformDirection(movimento);
        movimento.y = 0;

        // =========================
        // 2. CORRIDA (SHIFT)
        // =========================
        bool estaSeMovendo = movimento.magnitude > 0;
        bool estaCorrendo = estaSeMovendo && Input.GetKey(KeyCode.LeftShift);

        float velocidadeAtual = estaCorrendo ? velocidadeCorrer : velocidadeAndar;

        // =========================
        // 3. MOVIMENTO HORIZONTAL
        // =========================
        controller.Move(movimento.normalized * velocidadeAtual * Time.deltaTime);

        // rotação
        if (estaSeMovendo)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movimento),
                Time.deltaTime * 10
            );
        }

        // =========================
        // 4. ANIMAÇÃO ANDAR/CORRER
        // =========================
        animator.SetBool("Walking", estaSeMovendo);
        animator.SetBool("Running", estaCorrendo); // (ou "correr", se seu Animator usa esse nome)

        // =========================
        // 5. CHÃO
        // =========================
        estaNoChao = Physics.CheckSphere(peDoPlayer.position, 0.3f, colisaoLayer);
        animator.SetBool("onGround", estaNoChao);

        // =========================
        // 6. PULO
        // =========================
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            forcaY = 5f;
            animator.SetTrigger("Jumping");
        }

        // =========================
        // 7. GRAVIDADE
        // =========================
        forcaY += Physics.gravity.y * Time.deltaTime;

        controller.Move(new Vector3(0, forcaY, 0) * Time.deltaTime);
    }
}