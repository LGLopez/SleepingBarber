using System;
using System.Threading;

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

            Semaphore barberReady = new Semaphore(0, 1);                                            
            Semaphore accessWaitingRoomSeats = new Semaphore(1, 1);                                 // acceder al cuarto de espera
            Semaphore custReady = new Semaphore(0, 3);                                              // numero de clientes en el cuarto de espera

            Console.WriteLine("Todo preparado");

            void Barber()
            {
                while (!terminado)
                {
                    custReady.WaitOne();                                                            // Esperando ser despertado por un cliente
                    accessWaitingRoomSeats.WaitOne(); 

                    numberOfFreeWaitingRoomSeats++;

                    barberReady.Release();
                    
                    Console.WriteLine("El barbero corta el cabello de un cliente...");

                    accessWaitingRoomSeats.Release();
                    
                    Thread.Sleep(rand.Next(1, 5) * 1000);                                           // Espera a que otra persona tome la silla en caso de que haya alguien mas
                }
            }

            void Customer(Object number)
            {

                int num = (int)number;
                bool clientDone = false;
                while (!clientDone)                                                                 // El cliente volvera hasta que se le pueda atender
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

            Thread barberThread = new Thread(Barber);
            
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
        }
    }
}
