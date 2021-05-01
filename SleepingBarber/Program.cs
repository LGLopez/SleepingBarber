using System;
using System.Threading;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            Semaphore barberReady = new Semaphore(0, 1);
            Semaphore accessWaitingRoomSeats = new Semaphore(0, 1); // El numero de asientos puede ser incrementado o decrementado
            Semaphore custReady = new Semaphore(0, 3); // numero de clientes en el cuarto de espera

            int numberOfFreeWaitingRoomSeats = 10;
            int maxClients = 10;

            Console.WriteLine("Todo preparado");

            void Barber()
            {
                Console.WriteLine("Entro hilo barbero.");
                while (true)
                {
                    Console.WriteLine("Dentro del ciclo");
                    custReady.WaitOne();
                    accessWaitingRoomSeats.WaitOne();
                    numberOfFreeWaitingRoomSeats++;
                    barberReady.Release();
                    accessWaitingRoomSeats.Release();
                    Console.WriteLine("Cute hair here...");
                }
            }

            void Customer(Object number)
            {
                while (true)
                {
                    accessWaitingRoomSeats.WaitOne();

                    if (numberOfFreeWaitingRoomSeats > 0)
                    {
                        numberOfFreeWaitingRoomSeats--;
                        custReady.Release();
                        accessWaitingRoomSeats.Release();
                        barberReady.WaitOne();

                        Console.WriteLine("Have cut hair here");
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

            barberThread.Join();
            Console.WriteLine("Terminado!");
        }
    }
}
