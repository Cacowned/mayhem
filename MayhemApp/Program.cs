﻿using System;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;


namespace MayhemApp
{
    class Program
    {
        static Mayhem<ICli> mayhem;
        static void Main(string[] args)
        {
            //Properties.Settings.Default.RunListSettings = "";
            //Properties.Settings.Default.Save();

            /// TODO: Figure out how to load up the correct assemblies
            /// 

            if (File.Exists(Base64Serialize<Mayhem<ICli>>.filename)) {
               // Debug.WriteLine(Properties.Settings.Default.RunListSettings);
                
                try {
                    mayhem = Base64Serialize<Mayhem<ICli>>.Deserialize();

                    Console.WriteLine("Starting up with "+mayhem.ConnectionList.Count+" connections");
                
                } catch (SerializationException e) {
                    Debug.WriteLine("(De-)SerializationException " + e);
                }
            } else { 
                mayhem = new Mayhem<ICli>();
            }

            while (true) {
                Console.WriteLine();
                Console.WriteLine("0)\tView the run list");
                Console.WriteLine("1)\tCreate a connection");
                Console.WriteLine("2)\tExit");

                int number = validateNumber(2);
                Console.WriteLine();
                switch (number) {
                    case 0: runList();
                        break;
                    case 1: addConnection();
                        break;
                    case 2: 
                        Base64Serialize<Mayhem<ICli>>.SerializeObject(mayhem);
                        return;
                }
            }


        }

        public static void runList() {
            Console.WriteLine("Run List:");
            int numConnections = mayhem.ConnectionList.Count();
            if (numConnections == 0) {
                Console.WriteLine("There are no connections. Make one");
                return;
            }

            for (int i = 0; i < mayhem.ConnectionList.Count; i++) {
                var connection = mayhem.ConnectionList[i];
                Console.Write(i+")\t");
                Console.Write(printConnection(connection));
                if (connection.Enabled) {
                    Console.WriteLine(" - Running");
                } else {
                    Console.WriteLine(" - Stopped");
                }
            }

            Console.Write("Configure? Y/N: ");
            if (isYes()) {
                Console.WriteLine("Configure connection number: ");
                int num = validateNumber(mayhem.ConnectionList.Count - 1);
                var connection = mayhem.ConnectionList[num];

                Console.WriteLine("Configuring {0}", printConnection(connection));

                Console.WriteLine("Configure Action or Reaction? A/R: ");
                string input = Console.ReadLine();
                if (input.ToLower().Equals("a")) {
                    if (!connection.Action.HasConfig) {
                        Console.WriteLine("Action {0} can't be configured", connection.Action);
                    } else {
                        Console.WriteLine("Configuring {0}", connection.Action);
                        ((ICli)connection.Action).CliConfig();
                    }
                } else if (input.ToLower().Equals("r")) {
                    if (!connection.Reaction.HasConfig) {
                        Console.WriteLine("Reaction {0} can't be configured", connection.Reaction);
                    } else {
                        Console.WriteLine("Configuring {0}", connection.Reaction);
                        ((ICli)connection.Reaction).CliConfig();
                    }
                }
            } else {
                Console.Write("Start / Stop a connection? Y/N: ");
                if (isYes()) {
                    Console.Write("Which connection? ");
                    int num = validateNumber(numConnections - 1);
                    Connection connection = mayhem.ConnectionList[num];

                    // Flip whether it is enabled or not
                    if (connection.Enabled) {
                        connection.Disable();
                    } else {
                        connection.Enable();
                    }
                }
            }
        }

        public static void addConnection() {
            Console.WriteLine("Create a Connection:");
            ActionBase action = chooseAction();

            Console.WriteLine();
            ReactionBase reaction = chooseReaction();
           
            Connection conn = new Connection(action, reaction);

            mayhem.ConnectionList.Add(conn);

            Console.WriteLine("Created a new connection: {0}", printConnection(conn));
        }

        public static ActionBase chooseAction() {
            int numActions = mayhem.ActionList.Count();
            Console.WriteLine("Choose an Action:");
            for (int i = 0; i < numActions; i++) {
                Console.Write(i + ")\t");
                Console.WriteLine(mayhem.ActionList[i].Name);
            }

            int num = validateNumber(numActions-1);

            Type actionType = mayhem.ActionList[num].GetType();
            var action = Activator.CreateInstance(actionType);
            
            return (ActionBase)action;
        }

        public static ReactionBase chooseReaction() {
            int numReactions = mayhem.ReactionList.Count();
            Console.WriteLine("Choose a Reaction:");
            for (int i = 0; i < numReactions; i++) {
                Console.Write(i + ")\t");
                Console.WriteLine(mayhem.ReactionList[i].Name);
            }

            int num = validateNumber(numReactions-1);

            Type reactionType = mayhem.ReactionList[num].GetType();
            var reaction = Activator.CreateInstance(reactionType);

            return (ReactionBase)reaction;

        }

        public static int validateNumber(int maxNum) {
            int num = 0;
            string input = "";
            do {
                Console.Write("Number: ");
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out num) || (num < 0 || num > maxNum));
            return num;
        }

        public static string printConfig(ModuleBase module) {
            StringBuilder builder = new StringBuilder();
            
            if (module.HasConfig) {
                builder.Append("(");
                builder.Append("C");
                builder.Append(")");
            }
            /*else {
                Console.Write("-");
            }*/
            
            return builder.ToString();
        }

        public static string printConnection(Connection connection) {
            return String.Format("{0}{1} to {2}{3}", connection.Action.Name, printConfig(connection.Action),
                                                        connection.Reaction.Name, printConfig(connection.Reaction));
        }

        public static void clearScreen() {
            try{
                Console.Clear();
                welcomeMessage();
            }
            catch {}
        }

        public static void welcomeMessage() {
            Console.WriteLine("Welcome to Mayhem!");
            Console.WriteLine("Doing cool things with your computer.");
            Console.WriteLine();
        }

        public static bool isYes() {
            string input = Console.ReadLine();

            return input.Equals("y", StringComparison.OrdinalIgnoreCase);

        }
    }
}
