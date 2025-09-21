using System.Net; // основний простір імен для роботи з мережевими адресами та протоколами
using System.Net.Sockets; // простір імен для роботи з сокетами
using System.Text; // простір імен для роботи з кодуваннями

class Server // клас реалізує серверну логіку
{
    private const int DEFAULT_BUFLEN = 512; // задає розмір буфера для отримання даних
    // якщо потрібно працювати з великою кількістю даних, рекомендується використовувати буфери від 4 КБ до 64 КБ (розмір, з яким зазвичай працюють мережеві програми)
    // якщо дані невеликі та очікується, що вони приходитимуть у невеликих обсягах, можна використовувати буфер 512 байт або навіть менше
    private const string DEFAULT_PORT = "27015"; // вказує порт, на якому сервер прослуховуватиме підключення
    private const int PAUSE = 1000; // задає паузу в мілісекундах для краси та зручності виведення повідомлень (можна сміливо прибрати)

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8; // кирилиця
        Console.Title = "SERVER SIDE";
        Console.WriteLine("Процес сервера запущено!");
        //Thread.Sleep(PAUSE);

        try
        {
            var ipAddress = IPAddress.Any; // отримує будь-яку доступну IP-адресу для прослуховування (означає, що сервер слухатиме на всіх інтерфейсах, наприклад, Wi-Fi, Ethernet
            var localEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT)); // створює кінцеву точку (адресу та порт), до якої сервер буде прив’язаний

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // створює сокет для використання TCP-з’єднання (потоковий сокет)
            listener.Bind(localEndPoint); // прив’язує сокет до вказаної адреси та порту

            Console.WriteLine("Отримання адреси та порту сервера пройшло успішно!");
            //Thread.Sleep(PAUSE);

            listener.Listen(10); // починає прослуховування вхідних з’єднань, встановлюючи максимальну кількість з’єднань, що очікують (10), тобто сервер може мати до 10 клієнтів (з’єднань) у черзі на підключення
            Console.WriteLine("Починається прослуховування інформації від клієнта.\nБудь ласка, запустіть клієнтську програму!");

            var clientSocket = listener.Accept(); // очікує підключення клієнта та приймає його, повертаючи сокет для спілкування з клієнтом. є AcceptAsync(), щоб не блокувати потік
            Console.WriteLine("Підключення з клієнтською програмою встановлено успішно!");

            //listener.Close(); // закриває сокет слухача, оскільки з’єднання з клієнтом уже встановлено.
            // з’єднання з клієнтом тепер керується окремим сокетом, отриманим від методу Accept(), і слухаючий сокет більше не потрібен
            // АЛЕ! якщо сервер має обробляти кілька клієнтів одночасно або послідовно, сокет слухача НЕ закривають після першого Accept()
            // натомість listener.Accept() викликається в циклі або асинхронно, щоб приймати нові з’єднання, а кожен новий клієнт отримує свій власний clientSocket !!!

            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);

                if (bytesReceived > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Процес клієнта надіслав повідомлення: {message}");

                    if (int.TryParse(message, out int command))
                    {
                        if (command == 0)
                        {
                            Console.WriteLine("Клієнт завершив роботу.");
                            break;
                        }

                        string response;

                        if (command >= 1 && command <= 3)
                        {
                            response = command switch
                            {
                                1 => "не погано",
                                2 => "привіт, клієнт",
                                3 => "я TCP-сервер",
                                _ => "невідома команда"
                            };
                        }
                        else
                        {
                            int numberPlusOne = command + 1;
                            response = $"Ваше число + 1 = {numberPlusOne}";
                        }

                        clientSocket.Send(Encoding.UTF8.GetBytes(response));
                    }
                    else
                    {
                        string error = "треба вводити число";
                        clientSocket.Send(Encoding.UTF8.GetBytes(error));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
    }

}
