using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Enemy enemiesInRange;

    public void TakeDamage()
    {
        Debug.Log("Ouch!");
    }
}
