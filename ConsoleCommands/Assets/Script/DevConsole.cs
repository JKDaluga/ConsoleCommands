using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Console
{
    public class DevConsoleDictionaryKey
    {
        string key;
        public DevConsoleDictionaryKey(string str)
        {
            key = str;
        }

        public override bool Equals(object obj) {
            return Equals(obj as DevConsoleDictionaryKey);
        }

        public bool Equals(DevConsoleDictionaryKey obj)
        {
            return obj != null && obj.key.Equals(key);
        }

        public override int GetHashCode()
        {
            return 249886028 + EqualityComparer<string>.Default.GetHashCode(key);
        }

        public static bool operator ==(DevConsoleDictionaryKey lhs, DevConsoleDictionaryKey rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DevConsoleDictionaryKey lhs, DevConsoleDictionaryKey rhs)
        {
            
            return !(lhs == rhs);
        }
    }
        public class DevConsole : MonoBehaviour
    {
        public static DevConsole Instance { get; private set; }
        public static Dictionary<DevConsoleDictionaryKey, ConsoleCommand> Commands { get; private set; }

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
            Commands = new Dictionary<DevConsoleDictionaryKey, ConsoleCommand>();
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
            DevConsoleDictionaryKey name = new DevConsoleDictionaryKey(_name);
            if (!Commands.ContainsKey(name))
            {
                Commands.Add(name, _command);
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
            DevConsoleDictionaryKey name = new DevConsoleDictionaryKey(_input[0]);

            if (_input.Length == 0 || _input == null)
            {
                Debug.LogWarning("Command not recognized.");
                return;
            }

            if (!Commands.ContainsKey(name))
            {
                Debug.LogWarning("Command not recognized.");
            }
            else
            {
                Commands[name].RunCommand();
            }
        }
    }
}
