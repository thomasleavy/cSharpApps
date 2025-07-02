/*to compile: dotnet run*/
using System;
using System.Threading.Tasks;
using ToDoApp.Services;

namespace ToDoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new ToDoService("tasks.json");
            await service.LoadAsync();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== To-Do List ===");
                service.PrintTasks();
                Console.WriteLine("\n[A]dd  [M]ark Done  [R]emove  [Q]uit");
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.A:
                        Console.Write("New task: ");
                        service.AddTask(Console.ReadLine() ?? "");
                        await service.SaveAsync();
                        break;
                    case ConsoleKey.M:
                        Console.Write("Mark # to complete: ");
                        if (int.TryParse(Console.ReadLine(), out int m))
                            service.MarkComplete(m - 1);
                        await service.SaveAsync();
                        break;
                    case ConsoleKey.R:
                        Console.Write("Remove # task: ");
                        if (int.TryParse(Console.ReadLine(), out int r))
                            service.RemoveTask(r - 1);
                        await service.SaveAsync();
                        break;
                    case ConsoleKey.Q:
                        return;
                }
            }
        }
    }
}
