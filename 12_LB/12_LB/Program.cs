using System;
using System.Linq;
using System.Text;
using static EmailAndArrayProcessing.Program;

namespace EmailAndArrayProcessing
{
    class Program
    {
        
        public delegate void ArrayDelegate(double[] array, int index);
        public delegate void ArrayDelegate1(double[] array);
        public delegate string EmailDelegate(int Incoming,int Spam);
        public delegate string EmailDelegate1(int toDelete,int Incoming, int Spam);

        static void Main(string[] args)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Работа с массивом");
            Console.WriteLine("2 - Работа с электронной почтой");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    WorkWithArray();
                    break;
                case "2":
                    WorkWithEmail();
                    break;
                default:
                    Console.WriteLine("Ошибка: неверный выбор.");
                    return;
            }
        }

        static void WorkWithArray()
        {
            Console.Write("Введите длину массива: ");
            int length;
            while (!int.TryParse(Console.ReadLine(), out length) || length <= 0)
            {
                Console.WriteLine("Ошибка: введите положительное целое число.");
                Console.Write("Введите длину массива: ");
            }

            double[] array = new double[length];

            Console.WriteLine("Введите элементы массива:");

            for (int i = 0; i < length; i++)
            {
                while (!double.TryParse(Console.ReadLine(), out array[i]))
                {
                    Console.WriteLine("Ошибка: введите вещественное число.");
                    Console.Write("Введите элемент массива: ");
                }
            }

            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Сортировка массива");
            Console.WriteLine("2 - Обработка массива");
            string arrayChoice = Console.ReadLine();

            ArrayDelegate arrayDelegate = null;
            ArrayDelegate1 arrayDelegate1 = null;

            switch (arrayChoice)
            {
                case "1":
                    Console.Write("Введите индекс элемента для сравнения: ");
                    int index = int.Parse(Console.ReadLine());
                    arrayDelegate = SortArray;
                    arrayDelegate(array, index);
                    break;
                case "2":
                    arrayDelegate1 = ProcessArray;
                    arrayDelegate1(array);
                    break;
                default:
                    Console.WriteLine("Ошибка: неверный выбор.");
                    return;
            }
        }

        static void WorkWithEmail()
        {
            Console.Write("Введите количество входящих писем: ");
            int incoming = int.Parse(Console.ReadLine());

            Console.Write("Введите количество спам-писем: ");
            int spam = int.Parse(Console.ReadLine());

            Email email = new Email(incoming, spam);

            email.ManageEmails();
        }

        static void SortArray(double[] array, int index)
        {
            if (index < 0 || index >= array.Length)
            {
                Console.WriteLine("Ошибка: индекс вне диапазона.");
                return;
            }

            double pivot = array[index];
            double[] greater = Array.FindAll(array, element => element > pivot);
            double[] lesser = Array.FindAll(array, element => element <= pivot);

            Array.Sort(greater);
            Array.Sort(lesser);
            Array.Reverse(greater); 

            Console.WriteLine("Отсортированный массив:");
            double[] result = new double[greater.Length + lesser.Length];
            greater.CopyTo(result, 0);
            lesser.CopyTo(result, greater.Length);

            foreach (var num in result)
                Console.Write(num + " ");
            Console.WriteLine();
        }

        static void ProcessArray(double[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i + 1] != 0)
                    array[i] = (array[i] / array[i + 1]) * 100;
                else
                    array[i] = 0; 
            }

            Console.WriteLine("Обработанный массив:");
            foreach (var num in array)
                Console.Write(num + " ");
            Console.WriteLine();
        }
    }

    class Event
    {
        public string s(int Incoming,int Spam)
        {
            return $"Создана почта с {Incoming} входящими и {Spam} спамными письмами.";
        }
        public string s1(int toDelete,int Incoming, int Spam)
        {
            return $"Удалено {toDelete} писем. Осталось: {Incoming} входящих и {Spam} спамных.";
        }
    }
    class Email : Event
    {
        public event EmailDelegate EV;
        public event EmailDelegate1 EV1;
        public int Incoming { get; private set; }
        public int Spam { get; private set; }

        public Email(int incoming, int spam)
        {
            Incoming = incoming;
            Spam = spam;
            EV+= s;
            Console.WriteLine(EV(incoming,spam));
            
        }

        public void ManageEmails()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Ответить на письма");
                Console.WriteLine("2 - Удалить письма");
                Console.WriteLine("3 - Выход");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите количество отвеченных писем: ");
                        int toReply;
                        if (int.TryParse(Console.ReadLine(), out toReply) && toReply <= Incoming)
                        {
                            Incoming -= toReply;
                            Console.WriteLine($"Вы ответили на {toReply} писем. Осталось входящих: {Incoming}.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: неверное количество отвеченных писем.");
                        }
                        break;

                    case "2":
                        Console.Write("Введите количество удаляемых писем: ");
                        int toDelete;
                        if (int.TryParse(Console.ReadLine(), out toDelete) && toDelete <= (Incoming + Spam))
                        {
                            int total = Incoming + Spam;
                            if (toDelete <= Incoming)
                            {
                                Incoming -= toDelete;
                            }
                            else
                            {
                                toDelete -= Incoming;
                                Incoming = 0;
                                Spam -= toDelete;
                            }
                            EV1 += s1;
                            Console.WriteLine(EV1(toDelete, Incoming, Spam));
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: неверное количество удаляемых писем.");
                        }
                        break;

                    case "3":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Ошибка: неверный выбор.");
                        break;
                }
            }
        }
    }
}