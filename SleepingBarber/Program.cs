﻿using System;
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
            Semaphore accessWaitingRoomSeats = new Semaphore(0, 1); // la silla donde se corta el cabello
            Semaphore custReady = new Semaphore(0, 3); // numero de clientes en el cuarto de espera

            Console.WriteLine("Todo preparado");

            void Barber()
            {
                while (!terminado)
                {
                    custReady.WaitOne();
                    accessWaitingRoomSeats.WaitOne();

                    numberOfFreeWaitingRoomSeats++;

                    barberReady.Release();
                    accessWaitingRoomSeats.Release();
                    Console.WriteLine("Corte de cabello de un cliente...");
                }
            }

            void Customer(Object number)
            {
                int num = (int)number;

                Thread.Sleep(rand.Next(1, 5) * 1000);

                // Console.WriteLine("Dentro hilo cliente {0}", num);

                // accessWaitingRoomSeats.WaitOne();

                if (numberOfFreeWaitingRoomSeats > 0)
                {
                    numberOfFreeWaitingRoomSeats--;
                    custReady.Release();
                    accessWaitingRoomSeats.WaitOne();
                    barberReady.WaitOne();

                    Console.WriteLine("Se le corta el pelo al cliente{0}", num);
                }
                else
                {
                    accessWaitingRoomSeats.Release();

                    Console.WriteLine("Irse sin corte de cabello");
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
