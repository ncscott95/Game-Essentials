using UnityEngine;

public interface IMovementStrategy
{
    void Move(Transform transform);
}

public class WalkingStrategy : IMovementStrategy
{
    public void Move(Transform transform)
    {
        // Implement walking movement logic here
        Debug.Log("Walking...");
    }
}

public class FlyingStrategy : IMovementStrategy
{
    public void Move(Transform transform)
    {
        // Implement flying movement logic here
        Debug.Log("Flying...");
    }
}

public class Character : MonoBehaviour
{
    private IMovementStrategy _movementStrategy;

    public void SetMovementStrategy(IMovementStrategy movementStrategy)
    {
        _movementStrategy = movementStrategy;
    }

    private void Update()
    {
        if (_movementStrategy != null)
        {
            _movementStrategy.Move(transform);
        }
    }
}
