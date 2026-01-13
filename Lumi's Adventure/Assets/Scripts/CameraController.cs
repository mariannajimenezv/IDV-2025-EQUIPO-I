using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target; 

    [Header("Configuración")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f; 
    public Vector3 offset; // La distancia entre la cámara y el jugador

    void Start()
    {
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // la camara se mueve despues de que el jugador se haya movido
    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos dónde debería estar la cámara
        Vector3 desiredPosition = target.position + offset;

        // Nos movemos suavemente hacia esa posición e interpola
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicamos la posición
        transform.position = smoothedPosition;

        // Siempre mira al jugador
        transform.LookAt(target); 
    }
}
