using System;
using System.Threading;
using System.Text.RegularExpressions;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            bool terminado = false;                                                                 // En caso de que no lleguen mas clientes termina el ciclo del barbero
            Random rand = new Random();
            int numberOfFreeWaitingRoomSeats = 3;                                                   // Cantidad de asientos libres
            int maxClients = 10;                                                                    // Cantidadad maxima de clientes
            string cantidadClients;
            
            Console.WriteLine("Ingrese la cantidad de clientes que iran al barbero(limite = 100): ");
            cantidadClients = Console.ReadLine();

            Console.Clear();

            if (Regex.IsMatch(cantidadClients, @"^\d+$"))
            {
                maxClients = Int16.Parse(cantidadClients);
                if(maxClients > 100 || maxClients < 1)
                {
                    maxClients = 10;
                    Console.WriteLine("Se introdujo una cantidad mayor o menor al limite, la cantidad de limites será 10");
                }
            }
            else
            {
                Console.WriteLine("La entrada contenia valores diferentes a numeros, el valor de clientes será 10");
            }

            Semaphore barberReady = new Semaphore(0, 1);                                            
            Semaphore accessWaitingRoomSeats = new Semaphore(1, 1);                                 // acceder al cuarto de espera
            Semaphore custReady = new Semaphore(0, 3);                                              // numero de clientes en el cuarto de espera

            Console.WriteLine("Presione escape o espere a que termine de atender los clientes para terminar.\n");

            void Barber()
            {
                while (!terminado)
                {
                    custReady.WaitOne();                                                            // Esperando ser despertado por un cliente
                    accessWaitingRoomSeats.WaitOne(); 

                    numberOfFreeWaitingRoomSeats++;

                    barberReady.Release();
                    
                    Console.WriteLine("El barbero esta cortando el cabello de un cliente...");

                    accessWaitingRoomSeats.Release();
                    
                    Thread.Sleep(rand.Next(1, 5) * 1000);                                           // Espera a que otra persona tome la silla en caso de que haya alguien mas
                }
            }

            void Customer(Object number)
            {

                int num = (int)number;
                bool clientDone = false;
                while (!clientDone && !terminado)                                                                 // El cliente volvera hasta que se le pueda atender
                {
                    Thread.Sleep(rand.Next(1, 9) * 1000);

                    Console.WriteLine("llego hilo cliente {0}", num);
                    accessWaitingRoomSeats.WaitOne();

                    if (numberOfFreeWaitingRoomSeats > 0)                                           // En caso de tener asientos libres pueden pasar a la sala de espera
                    {
                        Console.WriteLine("Entro a la sala de espera el cliente {0}", num);
                        numberOfFreeWaitingRoomSeats--;
                        custReady.Release();                                                        // Indican que estan listos para cortarse el cabello 
                        accessWaitingRoomSeats.Release();

                        barberReady.WaitOne();                                                      // Esperan a que el barbero este listo para atenderlos

                        Console.WriteLine("Se le corta el pelo al cliente {0}", num);
                        clientDone = true;
                    }
                    else
                    {                                                                               // En caso de que no se tengan asientos disponibles el cliente se va
                        accessWaitingRoomSeats.Release();
                        Console.WriteLine("Irse sin corte de cabello");
                    }
                }
            }

            void stopCode()
            {
                ConsoleKeyInfo cki;
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                {
                    terminado = true;
                    Environment.Exit(0);
                }
                    
            }

            Thread stopCodeThread = new Thread(stopCode);
            Thread barberThread = new Thread(Barber);

            stopCodeThread.Start();
            barberThread.Start();

            Thread[] Clients = new Thread[maxClients];

            for (int i = 0; i < maxClients ; i++)
            {
                Clients[i] = new Thread(new ParameterizedThreadStart(Customer));
                Clients[i].Start(i);
            }
            
            for (int i = 0; i < maxClients; i++)
            {
                Clients[i].Join();
            }
            terminado = true;
            barberThread.Join();
            
            Console.WriteLine("El barbero vuelve a dormir!");
            Console.WriteLine("Presione ESC para salir");

            stopCodeThread.Join();
        }
    }
}
