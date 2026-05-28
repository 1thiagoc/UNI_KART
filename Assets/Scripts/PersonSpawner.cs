using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    [Tooltip(
        "Optional prefab with a Person component. If empty, the spawner will create simple cylinders at runtime."
    )]
    public GameObject personPrefab;
    public Transform[] spawnPoints;
    public Transform[] destinationPoints;

    [Header("Configurações de Animação")]
    [Tooltip("Animation Controller para quando o personagem estiver esperando na calçada.")]
    public RuntimeAnimatorController idleAnimatorController;
    
    [Tooltip("Animation Controller para quando o personagem estiver sentado dentro do carro.")]
    public RuntimeAnimatorController sitAnimatorController;

    [Tooltip("Spawn on Start (one person per spawn point).")]
    public bool spawnOnStart = true;

    void Start()
    {
        Debug.Log(
            $"PersonSpawner.Start spawnOnStart={spawnOnStart} spawnPoints={(spawnPoints == null ? 0 : spawnPoints.Length)}"
        );
        if (spawnOnStart)
            SpawnAll();
    }

    void CreateDestinationMarker(Transform dest)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
        marker.name = "DestMarker_" + dest.name;
        marker.transform.SetParent(dest);
        marker.transform.localPosition = new Vector3(0, 0.05f, 0);
        marker.transform.localRotation = Quaternion.Euler(90, 0, 0);
        marker.transform.localScale = new Vector3(6f, 6f, 1f);

        Destroy(marker.GetComponent<Collider>());

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        if (shader == null)
        {
            Debug.LogWarning("Nenhum shader compatível encontrado para o marcador de destino.");
            return;
        }

        var mat = new Material(shader);
        mat.color = new Color(0f, 0f, 1f, 0.4f);
        marker.GetComponent<Renderer>().material = mat;
    }

    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("PersonSpawner: no spawn points assigned.");
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var sp = spawnPoints[i];
            if (sp == null) continue;

            StopZone stopZone = sp.GetComponent<StopZone>();

            // Se for uma StopZone válida e tiver pontos na calçada, spawna um passageiro por ponto!
            if (stopZone != null && stopZone.sidewalkPoints != null && stopZone.sidewalkPoints.Length > 0)
            {
                foreach (Transform point in stopZone.sidewalkPoints)
                {
                    if (point == null) continue;
                    CreatePassenger(point.position, point.rotation, stopZone);
                }
            }
            else
            {
                // Fallback de segurança: se não for uma vaga com calçadas, spawna apenas um no centro do ponto
                CreatePassenger(sp.position, sp.rotation, stopZone);
            }
        }
    }

    private void CreatePassenger(Vector3 position, Quaternion rotation, StopZone sourceZone)
    {
        GameObject personGO;
        if (personPrefab != null)
        {
            personGO = Instantiate(personPrefab, position, rotation, null);
        }
        else
        {
            personGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            personGO.transform.position = position;
            personGO.transform.rotation = rotation;
            personGO.transform.localScale = new Vector3(0.5f, 1.0f, 0.5f);
            personGO.name = "Passenger";

            var r = personGO.GetComponent<Renderer>();
            if (r) r.material.color = Color.red;

            var col = personGO.GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }

        var person = personGO.GetComponent<Person>();
        if (person == null) 
            person = personGO.AddComponent<Person>();

        var animator = personGO.GetComponentInChildren<Animator>();
        if (animator == null && personPrefab != null)
        {
            // Se o prefab tiver um modelo 3D interno com Animator, tenta buscar
            animator = personGO.GetComponentInChildren<Animator>();
        }

        if (animator != null)
        {
            // Guarda as referências no script do personagem para uso futuro
            person.idleController = idleAnimatorController;
            person.sitController = sitAnimatorController;

            // Define o estado inicial como Idle (parado na calçada)
            if (idleAnimatorController != null)
            {
                animator.runtimeAnimatorController = idleAnimatorController;
            }
        }
        // Entrega a referência do collider da zona para o passageiro monitorar o embarque
        if (sourceZone != null)
        {
            person.myStopZoneCollider = sourceZone.GetComponent<Collider>();
        }

        // Define uma zona de destino aleatória baseada nas opções do array
        if (destinationPoints != null && destinationPoints.Length > 0)
        {
            person.destination = destinationPoints[Random.Range(0, destinationPoints.Length)];
        }
    }

    void OnDrawGizmos()
    {
        // Desenha os pontos de spawn (Verde) - ignora se já tiver o desenho interno do StopZone
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var sp in spawnPoints)
            {
                if (sp == null || sp.GetComponent<StopZone>() != null) continue;
                Gizmos.DrawSphere(sp.position, 0.2f);
                Gizmos.DrawLine(sp.position, sp.position + Vector3.up * 0.5f);
            }
        }

        // Desenha os pontos de destino (Ciano) - ignora se já tiver o desenho interno do StopZone
        if (destinationPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var dp in destinationPoints)
            {
                if (dp == null || dp.GetComponent<StopZone>() != null) continue;
                Gizmos.DrawCube(dp.position, Vector3.one * 0.3f);
                Gizmos.DrawLine(dp.position, dp.position + Vector3.up * 0.5f);
            }
        }
    }
}
