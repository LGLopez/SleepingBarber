using System;
using System.Threading;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            bool terminado = false;
            Random rand = new Random();
            int numberOfFreeWaitingRoomSeats = 3;
            int maxClients = 10;

            Semaphore barberReady = new Semaphore(0, 1); // Inicia en 0 por dormir
            Semaphore accessWaitingRoomSeats = new Semaphore(1, 1); // acceder al cuarto de espera
            Semaphore custReady = new Semaphore(0, 3); // numero de clientes en el cuarto de espera

            Console.WriteLine("Todo preparado");

            void Barber()
            {
                while (!terminado)
                {
                    custReady.WaitOne();
                    accessWaitingRoomSeats.WaitOne(); // quitando este jala :c pero no hace join

                    numberOfFreeWaitingRoomSeats++;

                    barberReady.Release();
                    accessWaitingRoomSeats.Release();
                    Console.WriteLine("Corte de cabello de un cliente...");
                }
            }

            void Customer(Object number)
            {

                int num = (int)number;
                bool clientDone = false;
                while (!clientDone)
                {
                    Console.WriteLine("llego hilo cliente {0}", num);

                    Thread.Sleep(rand.Next(1, 9) * 1000);
                    accessWaitingRoomSeats.WaitOne();

                    if (numberOfFreeWaitingRoomSeats > 0)
                    {
                        numberOfFreeWaitingRoomSeats--;
                        custReady.Release();
                        accessWaitingRoomSeats.Release();

                        barberReady.WaitOne();

                        Console.WriteLine("Se le corta el pelo al cliente {0}", num);
                        clientDone = true;
                    }
                    else
                    {
                        accessWaitingRoomSeats.Release();
                        Console.WriteLine("Irse sin corte de cabello");
                    }
                }

            }

            Thread barberThread = new Thread(Barber);
            Console.WriteLine("Hilo barbero creado.");

            barberThread.Start();

            Console.WriteLine("Hilo barbero iniciado.");

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
            Console.WriteLine("Terminado!");
        }
    }
}
