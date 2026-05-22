using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class JumpCommand : ICommand
{
    private Rigidbody _rigidbody;
    private float _jumpForce;

    public JumpCommand(Rigidbody rigidbody, float jumpForce)
    {
        _rigidbody = rigidbody;
        _jumpForce = jumpForce;
    }

    public void Execute()
    {
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    public void Undo()
    {
        _rigidbody.AddForce(Vector3.down * _jumpForce, ForceMode.Impulse);
    }
}
