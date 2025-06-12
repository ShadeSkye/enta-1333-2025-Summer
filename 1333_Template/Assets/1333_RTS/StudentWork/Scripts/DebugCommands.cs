using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class DebugCommands : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnEnable()
    {
        DebugLogConsole.AddCommand("HelloWorld", description: "prints a message to the console", HelloWorld);
    }

    private void OnDisable()
    {
        DebugLogConsole.RemoveCommand("HelloWorld");
    }

    private void HelloWorld()
    {
        Debug.Log("Hello world");
    }
}
