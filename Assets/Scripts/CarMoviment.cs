using UnityEngine;
using UnityEngine.InputSystem;

public class CarMoviment : MonoBehaviour
{
    public Rigidbody sphereRB;
    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    public float maxSpeed = 50f;
    public float brakeForce = 15f;
    public float turnStrength = 180f;
    public Transform carVisual;

    [Header("Pivots das Rodas")]
    public Transform wheelFrontLeft;
    public Transform wheelFrontRight;
    public Transform[] allWheels;

    [Header("Configurações da rota")]
    public float maxSteerAngle = 3f;
    public float wheelRotationSpeed = 500f;
    public float steerSmoothingSpeed = 8f;
    private float smoothedSteerAngle = 0f;
    
    // Configurações de arrasto (Drag)
    public float normalDrag = 0f;
    public float brakeDrag = 5f;

    // Input System
    private InputAction inputAction;
    private InputAction brakeAction;
    
    // Valores de input
    private Vector2 moviment;
    private float speedInput;
    private bool isBraking;

    void Awake()
    {
        inputAction = InputSystem.actions.FindAction("Move");
        brakeAction = InputSystem.actions.FindAction("Jump");
    }

    void Start()
    {
        // Desacopla a esfera para que ela role livremente
        sphereRB.transform.parent = null;
        normalDrag = sphereRB.linearDamping;
    }

    void Update()
    {
        // Input de acelerar/frear
        moviment = inputAction.ReadValue<Vector2>();
        isBraking = brakeAction.IsPressed();
        speedInput = isBraking ? 0 : moviment.y * (moviment.y > 0 ? forwardAccel : reverseAccel);

        // Lógica de Rotação por Inércia
        // Verificamos a magnitude da velocidade física, não o input do teclado
        float currentSpeed = sphereRB.linearVelocity.magnitude;
        float velocityDirection = Vector3.Dot(sphereRB.linearVelocity, transform.forward);
        if (currentSpeed > 0.1f)
        {
            // Se a velocidade for positiva, gira normal. Se for negativa (ré), inverte o giro.
            float multiplier = velocityDirection > 0 ? 1f : -1f;
            float rotationAmount = moviment.x * turnStrength * Time.deltaTime * multiplier;

            // estilo drift
            if(isBraking) rotationAmount *= 1.15f;

            transform.Rotate(0, rotationAmount, 0, Space.World);
        }

        // Faz o carro visual seguir a esfera
        transform.position = sphereRB.transform.position;
        MoveWheels();
    }

    void MoveWheels()
    {
        float targetSteerAngle = moviment.x * maxSteerAngle;
        smoothedSteerAngle = Mathf.MoveTowards(smoothedSteerAngle, targetSteerAngle, steerSmoothingSpeed * maxSteerAngle * Time.deltaTime);

        if (wheelFrontLeft != null) {
            wheelFrontLeft.localRotation = Quaternion.Euler(0, 0, smoothedSteerAngle);
        }
        if (wheelFrontRight != null) {
            wheelFrontRight.localRotation = Quaternion.Euler(0, 0, smoothedSteerAngle);
        }
    }

    void FixedUpdate()
    {
        // Gerenciamento do Arrasto (Drag)
        sphereRB.linearDamping = isBraking ? brakeDrag : normalDrag;

        float currentSpeed = sphereRB.linearVelocity.magnitude;
    
        if (currentSpeed > 0.5f && !isBraking)
        {
            // Descobre para onde o carro está se movendo (Frente ou Ré)
            float velocityDirection = Vector3.Dot(sphereRB.linearVelocity, transform.forward);
            Vector3 targetDirection = velocityDirection > 0 ? transform.forward : -transform.forward;

            // Forçamos a velocidade da esfera a se alinhar com a nova frente do carro.
            // O valor 0.1f controla a "derrapagem". Valores maiores (ex: 0.2f) fazem o carro grudar mais no chão.
            sphereRB.linearVelocity = Vector3.Lerp(sphereRB.linearVelocity, targetDirection * currentSpeed, 0.12f);
        }
        if (isBraking)
        {
            // Aplica uma força contrária à velocidade atual (Freio Ativo)
            sphereRB.AddForce(-sphereRB.linearVelocity * brakeForce, ForceMode.Acceleration);
            return;
        }
        // Verificamos se há input e se ainda está abaixo do limite de velocidade
        bool canAccelerate = Mathf.Abs(speedInput) > 0 && sphereRB.linearVelocity.magnitude < maxSpeed;
        if (canAccelerate)
        {
            // Aplica força para frente
            sphereRB.AddForce(transform.forward * speedInput * 10f, ForceMode.Acceleration);
        }
        // Trava de Velocidade (Hard Cap)
        if (sphereRB.linearVelocity.magnitude > maxSpeed)
        {
            sphereRB.linearVelocity = sphereRB.linearVelocity.normalized * maxSpeed;
        }
    }
}
