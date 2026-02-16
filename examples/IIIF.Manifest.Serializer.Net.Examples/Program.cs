using System;
using IIIF.Manifests.Serializer.Examples.Examples;

namespace IIIF.Manifests.Serializer.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     IIIF Manifest Serializer .NET - Examples              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (args.Length > 0)
            {
                RunExample(args[0]);
            }
            else
            {
                ShowMenu();
            }
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\nAvailable Examples:");
                Console.WriteLine("  1. Single Image Manifest");
                Console.WriteLine("  2. Multi-Page Book with Deep Zoom");
                Console.WriteLine("  3. Manifest with Hierarchical Structure");
                Console.WriteLine("  4. Deserialize and Modify Manifest");
                Console.WriteLine("  5. Collection with Viewing Hints");
                Console.WriteLine("  6. Run All Examples");
                Console.WriteLine("  0. Exit");
                Console.WriteLine();
                Console.Write("Select an example (0-6): ");

                var input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        SingleImageExample.Run();
                        break;
                    case "2":
                        BookManifestExample.Run();
                        break;
                    case "3":
                        StructuredManifestExample.Run();
                        break;
                    case "4":
                        DeserializeExample.Run();
                        break;
                    case "5":
                        CollectionExample.Run();
                        break;
                    case "6":
                        RunAllExamples();
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }

                if (input != "5")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void RunExample(string exampleName)
        {
            switch (exampleName.ToLower())
            {
                case "single":
                case "1":
                    SingleImageExample.Run();
                    break;
                case "book":
                case "2":
                    BookManifestExample.Run();
                    break;
                case "structure":
                case "3":
                    StructuredManifestExample.Run();
                    break;
                case "deserialize":
                case "4":
                    DeserializeExample.Run();
                    break;
                case "collection":
                case "5":
                    CollectionExample.Run();
                    break;
                case "all":
                case "6":
                    RunAllExamples();
                    break;
                default:
                    Console.WriteLine($"Unknown example: {exampleName}");
                    Console.WriteLine("Available: single, book, structure, deserialize, collection, all");
                    break;
            }
        }

        static void RunAllExamples()
        {
            SingleImageExample.Run();
            Console.WriteLine("\n" + new string('=', 60) + "\n");

            BookManifestExample.Run();
            Console.WriteLine("\n" + new string('=', 60) + "\n");

            StructuredManifestExample.Run();
            Console.WriteLine("\n" + new string('=', 60) + "\n");

            DeserializeExample.Run();
            Console.WriteLine("\n" + new string('=', 60) + "\n");

            CollectionExample.Run();
            Console.WriteLine("\n" + new string('=', 60) + "\n");

            Console.WriteLine("All examples completed!");
        }
    }
}

