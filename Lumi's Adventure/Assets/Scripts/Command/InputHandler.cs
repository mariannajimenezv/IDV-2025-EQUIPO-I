using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Referencias")]
    public LumiController lumi;      // El Receiver
    public CommandManager manager;   // El Invoker

    void Update()
    {
        HandleMovement();
        HandleCombat();
        HandleJump();
        HandleUndo();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            Vector3 direction = new Vector3(h, 0, v).normalized;
            ICommand moveCommand = new CommandMover(lumi, direction);

            manager.AddCommand(moveCommand, false);
        }
        else
        {
            manager.AddCommand(new CommandMover(lumi, Vector3.zero), false);
        }
    }

    void HandleCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ICommand attackCommand = new CommandAtacar(lumi);
            manager.AddCommand(attackCommand, true);
        }
    }

    void HandleJump() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ICommand jumpCommand = new CommandJump(lumi);
            manager.AddCommand(jumpCommand);
        }
    }

    void HandleUndo()
    {
        // tecla Z para deshacer acciones (prueba del sistema)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            manager.UndoLastCommand();
        }
    }
}
