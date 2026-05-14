using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    [Tooltip(
        "Optional prefab with a Person component. If empty, the spawner will create simple cylinders at runtime."
    )]
    public GameObject personPrefab;
    public Transform[] spawnPoints;
    public Transform[] destinationPoints;

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
            if (sp == null)
                continue;

            GameObject personGO;
            if (personPrefab != null)
            {
                personGO = Instantiate(personPrefab, sp.position, sp.rotation, null);
            }
            else
            {
                personGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                personGO.transform.position = sp.position;
                personGO.transform.rotation = sp.rotation;
                personGO.transform.localScale = new Vector3(0.5f, 1.0f, 0.5f);
                personGO.name = "Person";
                var r = personGO.GetComponent<Renderer>();
                if (r)
                    r.material.color = Color.red;
                // remove collider if you prefer no physics interactions
                var col = personGO.GetComponent<Collider>();
                if (col)
                    col.isTrigger = true;
            }

            var person = personGO.GetComponent<Person>();
            if (person == null)
                person = personGO.AddComponent<Person>();

            if (destinationPoints != null && destinationPoints.Length > 0)
            {
                person.destination = destinationPoints[Random.Range(0, destinationPoints.Length)];
            }
        }
        if (destinationPoints != null)
        {
            foreach (var dp in destinationPoints)
            {
                if (dp != null)
                    CreateDestinationMarker(dp);
            }
        }
    }

    void OnDrawGizmos()
    {
        // draw spawn points (green)
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var sp in spawnPoints)
            {
                if (sp == null)
                    continue;
                Gizmos.DrawSphere(sp.position, 0.2f);
                Gizmos.DrawLine(sp.position, sp.position + Vector3.up * 0.5f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(sp.position + Vector3.up * 0.5f, sp.name);
#endif
            }
        }

        // draw destination points (cyan)
        if (destinationPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var dp in destinationPoints)
            {
                if (dp == null)
                    continue;
                Gizmos.DrawCube(dp.position, Vector3.one * 0.3f);
                Gizmos.DrawLine(dp.position, dp.position + Vector3.up * 0.5f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(dp.position + Vector3.up * 0.5f, dp.name);
#endif
            }
        }
    }
}
