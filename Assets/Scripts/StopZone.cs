using UnityEngine;

public class StopZone : MonoBehaviour
{
    [Header("Configurações da Estação")]
    public bool isSpawnZone = true;       // Se marcado, é ponto de embarque. Se falso, é de desembarque.
    public Transform sidewalkPoint;       // Ponto na CALÇADA onde o passageiro fica/surge físico.
    public Vector3 cubeSize = Vector3.one;

    private void OnDrawGizmos()
    {
        // Desenha uma caixa verde na pista no editor para você ver o tamanho da vaga
        Gizmos.color = isSpawnZone ? new Color(0, 1, 0, 0.3f) : new Color(0, 0, 1, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, cubeSize);
        
        // Desenha uma linha até a calçada
        if (sidewalkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, sidewalkPoint.position);
            Gizmos.DrawSphere(sidewalkPoint.position, 0.2f);
        }
    }
}
