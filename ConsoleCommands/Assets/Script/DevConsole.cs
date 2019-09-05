using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Console
{
    public class DevConsole : MonoBehaviour
    {
        public static DevConsole Instance { get; private set; }
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        [Header("UI Components")]
        public Canvas consoleCanvas;
        public TextMeshProUGUI consoleText;
        public TextMeshProUGUI inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if(Instance != null)
            {
                return;
            }

            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();
        }

        private void Start()
        {
            consoleCanvas.gameObject.SetActive(false);
            CreateCommands();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string _message = "[" + type.ToString() + "] " + logMessage;
            AddMessageToConsole(_message);
        }

        private void CreateCommands()
        {
            CommandQuit.CreateCommand();
        }

        public static void AddCommandsToConsole(string _name, ConsoleCommand _command)
        {
            if(!Commands.ContainsKey(_name))
            {
                Commands.Add(_name, _command);
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.BackQuote))
            {
                consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);
            }

            if(consoleCanvas.gameObject.activeInHierarchy)
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    if(inputText.text != "")
                    {
                        AddMessageToConsole(inputText.text);
                        ParseInput(inputText.text);
                    }
                }
            }
        }

        private void AddMessageToConsole(string msg)
        {
            consoleText.text += msg + "\n";
        }

        private void ParseInput(string input)
        {
            string[] _input = input.Split(null);

            if (_input.Length == 0 || _input == null)
            {
                Debug.LogWarning("Command not recognized.");
                return;
            }

            if (!Commands.ContainsKey(_input[0]))
            {
                Debug.LogWarning("Command not recognized.");
            }
            else
            {
                Commands[_input[0]].RunCommand();
            }
        }
    }
}
