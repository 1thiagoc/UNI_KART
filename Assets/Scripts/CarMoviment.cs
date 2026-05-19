using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CarMoviment : MonoBehaviour
{
    public Rigidbody sphereRB;
    public TMP_Text speedText;
    
    [Header("Velocidade")]
    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    public float maxSpeed = 50f;
    public float brakeForce = 15f;
    public float turnStrength = 180f;

    [Header("Física Extra Anti-Pulo")]
    public float extraGravity = 30f; // Força para puxar o carro para baixo
    public float groundCheckDistance = 0.5f; // Distância para saber se está perto do chão
    public LayerMask groundLayer; // Camada das pistas/ruas
    public float normalDrag = 0f;

    [Header("Sistema de Drift & Boost")]
    public float driftTurnMultiplier = 1.6f; // O carro vira MAIS rápido durante o drift
    [Range(0f, 1f)] public float driftGrip = 0.04f; // Menor = desliza mais de lado (estilo gelo)
    [Range(0f, 1f)] public float normalGrip = 0.15f; // Maior = gruda mais no chão
    public float boostForce = 25f; // Força do turbo ao soltar o botão
    public float boostDuration = 0.5f; // Quanto tempo dura o turbo
    private float boostTimer;

    [Header("Pivots das Rodas")]
    public Transform wheelFrontLeft;
    public Transform wheelFrontRight;
    public Transform[] allWheels;

    [Header("Configurações da rota")]
    public float maxSteerAngle = 3f;
    public float wheelRotationSpeed = 500f;
    public float steerSmoothingSpeed = 8f;
    private float smoothedSteerAngle = 0f;

    [Header("Efeitos Visuais do Drift")]
    public Transform carVisual; // O modelo 3D do carro (filho deste objeto)
    public float driftVisualTilt = 15f; // Ângulo de inclinação do carro no drift
    public ParticleSystem[] wheelParticles; // Arraste os Particle Systems das rodas traseiras para cá
    public Color normalDriftColor = Color.yellow;
    public Color turboReadyColor = Color.cyan; // Cor quando o turbo estiver carregado

    // Input System
    private InputAction inputAction;
    private InputAction driftAction;
    
    // Valores de input
    private Vector2 moviment;
    private float speedInput;
    private bool isDrifting;
    private float driftTimer; // Conta quanto tempo você ficou em drift para calcular o nível do turbo

    void Awake()
    {
        inputAction = InputSystem.actions.FindAction("Move");
        driftAction = InputSystem.actions.FindAction("Jump");
    }

    void Start()
    {
        // Desacopla a esfera para que ela role livremente
        sphereRB.transform.parent = null;
        normalDrag = sphereRB.linearDamping;
        SetParticleStatus(false); // Desliga as partículas no início
    }

    void Update()
    {
        // Input de acelerar/frear
        moviment = inputAction.ReadValue<Vector2>();
        // Se o botão está pressionado E o carro está correndo rápido, ativa o Drift
        bool capotandoCheck = sphereRB.linearVelocity.magnitude > 5f;
        if (driftAction.IsPressed() && capotandoCheck && Mathf.Abs(moviment.x) > 0.1f)
        {
            isDrifting = true; // Ativou o drift neste frame
            SetParticleStatus(true);
            UpdateParticleEffects();
        }
        else
        {
            // Se soltou o botão e estava em drift, solta o Mini-Turbo!
            if (isDrifting)
            {
                TriggerMiniTurbo();
            }
        }

        speedInput = moviment.y * (moviment.y > 0 ? forwardAccel : reverseAccel);

        // Lógica de Rotação por Inércia
        // Verificamos a magnitude da velocidade física, não o input do teclado
        float currentSpeed = sphereRB.linearVelocity.magnitude;
        float velocityDirection = Vector3.Dot(sphereRB.linearVelocity, transform.forward);
        if (currentSpeed > 0.1f)
        {
            // Se a velocidade for positiva, gira normal. Se for negativa (ré), inverte o giro.
            float multiplier = velocityDirection > 0 ? 1f : -1f;
            float currentTurnStrength = isDrifting ? turnStrength * driftTurnMultiplier : turnStrength;
            
            float rotationAmount = moviment.x * currentTurnStrength * Time.deltaTime * multiplier;
            transform.Rotate(0, rotationAmount, 0, Space.World);
        }

        // Cronômetros
        if (isDrifting) driftTimer += Time.deltaTime;
        if (boostTimer > 0) boostTimer -= Time.deltaTime;

        transform.position = sphereRB.transform.position;
        // Aplica os efeitos visuais de inclinação de chassis
        HandleVisualTilt();
    
        MoveWheels();
    }

    void HandleVisualTilt()
    {
        // Se estiver em drift, inclina o chassis para o lado oposto ou a favor da curva
        float targetTilt = 0f;
        if (isDrifting)
        {
            // Multiplica pelo sinal do input para inclinar para o lado certo da curva
            targetTilt = -Mathf.Sign(moviment.x) * driftVisualTilt;
        }

        // 1. Pegamos a rotação atual em Z usando um método seguro que evita bugs de 360 graus na Unity
        float currentTiltZ = carVisual.localRotation.eulerAngles.z;
        if (currentTiltZ > 180) currentTiltZ -= 360; // Converte escala 0-360 para -180 a 180

        // 2. Suaviza a inclinação para não ser um tranco seco
        float smoothedTilt = Mathf.MoveTowards(currentTiltZ, targetTilt, 90f * Time.deltaTime);
        
        // 3. Forçamos X e Y a ficarem estritamente zerados em relação ao pai. 
        // Isso impede o carro visual de girar igual um pião sozinho!
        carVisual.localRotation = Quaternion.Euler(carVisual.localRotation.x, carVisual.localRotation.y, smoothedTilt);
    }

    void SetParticleStatus(bool playing)
    {
        foreach (var p in wheelParticles)
        {
            if (p == null) continue;
            if (playing && !p.isPlaying) p.Play();
            if (!playing && p.isPlaying) p.Stop();
        }
    }

    void UpdateParticleEffects()
    {
        // Altera a cor das partículas baseado no tempo de drift
        Color currentColor = driftTimer > 0.6f ? turboReadyColor : normalDriftColor;
        
        foreach (var p in wheelParticles)
        {
            if (p == null) continue;
            var mainModule = p.main;
            mainModule.startColor = currentColor;
        }
    }

    void FixedUpdate()
    {
        if (speedText != null)
        {
            // Pega a velocidade real da esfera física
            float carSpeed = sphereRB.linearVelocity.magnitude;

            // Multiplica por um fator se quiser converter para KM/H visual (ex: * 3.6f)
            // Se a sua velocidade máxima já estiver calibrada em um valor alto (ex: 50f), pode usar o valor direto.
            int speedInKmh = Mathf.RoundToInt(carSpeed * 2.5f); // Ajuste o multiplicador (2.5f) para parecer realista na sua pista
            speedText.text = "Velocidade: " + speedInKmh + " KM/H";
        }
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (isGrounded)
        {
            // Se estiver colado na pista, empurra para baixo com força extra para ignorar mini-fendas
            sphereRB.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
        else
        {
            // Se saltar de verdade (uma rampa real), aplica uma gravidade menor ou padrão
            sphereRB.AddForce(Vector3.down * (extraGravity * 2f), ForceMode.Acceleration);
        }
        sphereRB.linearDamping = normalDrag;
        float currentSpeed = sphereRB.linearVelocity.magnitude;
    
        // 2. A MÁGICA DO DRIFT (Ajuste de Grip)
        if (currentSpeed > 0.5f)
        {
            float velocityDirection = Vector3.Dot(sphereRB.linearVelocity, transform.forward);
            Vector3 targetDirection = velocityDirection > 0 ? transform.forward : -transform.forward;

            // Se estiver em drift, usamos 'driftGrip' (baixo, faz deslizar). Se não, usamos 'normalGrip' (alto, gruda).
            float currentGrip = isDrifting ? driftGrip : normalGrip;
            sphereRB.linearVelocity = Vector3.Lerp(sphereRB.linearVelocity, targetDirection * currentSpeed, currentGrip);
        }

        // 3. Sistema de Aceleração / Turbo
        float currentAccel = speedInput;
        float currentMaxSpeed = maxSpeed;

        // Se o turbo estiver ativo, aumenta a aceleração e ultrapassa o limite de velocidade máxima!
        if (boostTimer > 0)
        {
            currentAccel = forwardAccel * 2f;
            currentMaxSpeed = maxSpeed * 1.5f;
            sphereRB.AddForce(transform.forward * boostForce, ForceMode.VelocityChange);
        }

        bool canAccelerate = Mathf.Abs(currentAccel) > 0 && sphereRB.linearVelocity.magnitude < currentMaxSpeed;
        if (canAccelerate)
        {
            sphereRB.AddForce(transform.forward * currentAccel * 10f, ForceMode.Acceleration);
        }

        // Trava de Velocidade Dinâmica
        if (sphereRB.linearVelocity.magnitude > currentMaxSpeed)
        {
            sphereRB.linearVelocity = sphereRB.linearVelocity.normalized * currentMaxSpeed;
        }
    }

    void TriggerMiniTurbo()
    {
        isDrifting = false;
        SetParticleStatus(false);

        // Só ganha turbo se segurou o drift por mais de 0.6 segundos (evita ganhar turbo só clicando rápido)
        if (driftTimer > 0.6f)
        {
            boostTimer = boostDuration;
            Debug.Log("MINI TURBO ACTIVATED!");
            // Aqui você pode instanciar partículas de fogo azul/laranja nas rodas!
        }

        driftTimer = 0f;
    }

    void MoveWheels()
    {
        float targetSteerAngle = moviment.x * maxSteerAngle;
        smoothedSteerAngle = Mathf.MoveTowards(smoothedSteerAngle, targetSteerAngle, steerSmoothingSpeed * maxSteerAngle * Time.deltaTime);

        if (wheelFrontLeft != null) {
            wheelFrontLeft.localRotation = Quaternion.Euler(0, smoothedSteerAngle, 0);
        }
        if (wheelFrontRight != null) {
            wheelFrontRight.localRotation = Quaternion.Euler(0, smoothedSteerAngle, 0);
        }
    }
}
