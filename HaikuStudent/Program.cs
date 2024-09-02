using System;
using System.Collections.Generic;

namespace HaikuStudent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter hostname and a list of ports (space separated) to connect to HaikuMasters:");
            Console.WriteLine("Example: localhost 5000 5001 5002");

            var input = Console.ReadLine()?.Split(' ');
            if (input == null || input.Length < 2)
            {
                Console.WriteLine("Invalid input. Please provide a hostname and at least one port.");
                return;
            }

            string hostname = input[0];
            List<int> ports = new List<int>();
            for (int i = 1; i < input.Length - 1; i++)
            {
                if (int.TryParse(input[i], out int port))
                {
                    ports.Add(port);
                }
            }

            string librarianPath = input[input.Length-1];

            if (ports.Count == 0)
            {
                Console.WriteLine("No valid ports provided.");
                return;
            }

            HaikuStudentClient studentClient = new HaikuStudentClient(hostname, ports, librarianPath);
            studentClient.Start();

            Console.WriteLine("Press any key to stop the Student...");
            Console.ReadKey();

            studentClient.Stop();
        }
    }
}
