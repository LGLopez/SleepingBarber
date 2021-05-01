using System;
using System.Threading;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            int cantidadSillas = 3;
            int maximoClientes = 10;
            bool listo = false;

            Semaphore salaEspera = new Semaphore(cantidadSillas, cantidadSillas);
            Semaphore sillaBarbero = new Semaphore(0, 1);
            Semaphore barberoListo = new Semaphore(0, 1);
            Semaphore entrarSala = new Semaphore(0, 1);
            
            void barbero()
            {
                while (!listo)
                {

                    salaEspera.WaitOne();
                    entrarSala.WaitOne();
                    cantidadSillas += 1;
                    barberoListo.Release();
                    entrarSala.Release();

                    Console.WriteLine("Corta cabello");
                }
            }

            void cliente(Object number)
            {
                while (true)
                {
                    entrarSala.WaitOne();

                    if (cantidadSillas > 0)
                    {
                        cantidadSillas -= 1;
                        salaEspera.Release();
                        entrarSala.Release();
                        barberoListo.WaitOne();

                        Console.WriteLine("have a hair cut here");
                    }
                    else
                    {
                        entrarSala.Release();
                        Console.WriteLine("Se ve sin corte de pelo");
                    }
                }
            }

            Thread HiloBarbero = new Thread(barbero);
            HiloBarbero.Start();

            Thread[] Clientes = new Thread[maximoClientes];

            for(int i = 0; i < maximoClientes; i++)
            {
                Clientes[i] = new Thread(new ParameterizedThreadStart(cliente));
                Clientes[i].Start();
            }

            for (int i = 0; i < maximoClientes; i++)
            {
                Clientes[i].Join();
            }
            listo = true;
            barberoListo.Release();

            HiloBarbero.Join();
            Console.WriteLine("Terminado!");
        }
    }
}
