using UnityEngine;

public class StopZone : MonoBehaviour
{
    [Header("Configurações da Estação")]
    public bool isSpawnZone = true;       // Se marcado, é ponto de embarque. Se falso, é de desembarque.
    
    [Tooltip("Adicione múltiplos pontos da calçada para esta vaga.")]
    public Transform[] sidewalkPoints;    // Array com múltiplos pontos na calçada

    public Vector3 cubeSize = Vector3.one;

    /// <summary>
    /// Retorna um ponto aleatório da lista de calçadas disponíveis.
    /// </summary>
    public Transform GetRandomSidewalkPoint()
    {
        if (sidewalkPoints == null || sidewalkPoints.Length == 0) return null;
        return sidewalkPoints[Random.Range(0, sidewalkPoints.Length)];
    }

    private void OnDrawGizmos()
    {
        // Desenha uma caixa na pista no editor para você ver o tamanho da vaga
        Gizmos.color = isSpawnZone ? new Color(0, 1, 0, 0.3f) : new Color(0, 0, 1, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, cubeSize);
        
        // Reseta a matriz para desenhar as linhas no espaço mundial corretamente
        Gizmos.matrix = Matrix4x4.identity;

        // Desenha uma linha até CADA ponto da calçada configurado
        if (sidewalkPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var point in sidewalkPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawLine(transform.position, point.position);
                    Gizmos.DrawSphere(point.position, 0.2f);
                }
            }
        }
    }
}
