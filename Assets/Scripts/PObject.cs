using UnityEngine;

public class PObject : MonoBehaviour
{
    public int hp = 5;

    public void Hit()
    {
        hp--;
        Debug.Log($"{name} Hit! HP: {hp}");

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
