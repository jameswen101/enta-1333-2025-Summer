using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class DebugCommands : MonoBehaviour
{
    private void OnEnable()
    {
        //string name, string description, function name
        DebugLogConsole.AddCommand("HelloWorld", "Prints a message to the console", HelloWorld);
    }

    private void HelloWorld()
    {
        Debug.Log("Hello world");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
