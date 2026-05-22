using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    
}

public class Zombie : Enemy
{
    
}

public class Ghost : Enemy
{
    
}

public static class EnemyFactory
{
    public static Enemy CreateEnemy(string type)
    {
        switch (type)
        {
            case "Zombie":
                return new GameObject("Zombie").AddComponent<Zombie>();
            case "Ghost":
                return new GameObject("Ghost").AddComponent<Ghost>();
            default:
                Debug.LogError("Unknown enemy type: " + type);
                return null;
        }
    }
}
