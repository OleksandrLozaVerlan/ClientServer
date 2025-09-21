using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Console.Title = "CLIENT SIDE";
        try
        {
            var ipAddress = IPAddress.Loopback; // IP-адреса локального хоста (127.0.0.1), яка використовується для підключення до сервера на поточному пристрої
            var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(remoteEndPoint); // ініціює підключення клієнта до сервера за вказаною кінцевою точкою (IP-адреса та порт)
            Console.WriteLine("Підключення до сервера встановлено.");

            Console.WriteLine("\nДоступні команди");
            Console.WriteLine("1 - як справи");
            Console.WriteLine("2 - привіт");
            Console.WriteLine("3 - хто ти");
            Console.WriteLine("0 - завершення\n");
            Console.WriteLine("якщо більше 3 то + 1 к числу");

            while (true)
            {
                Console.Write("Введіть число: ");
                string? message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(messageBytes);

                if (message == "0")
                    break;

                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Відповідь від сервера: {response}");
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine("З’єднання з сервером закрито.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
    }
}