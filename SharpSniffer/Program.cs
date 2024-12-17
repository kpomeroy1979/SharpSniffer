using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        
        Task tcpTask = Task.Run(() => StartTcpListener());
        Task udpTask = Task.Run(() => StartUdpListener());

        
        Task.WaitAll(tcpTask, udpTask);
    }

    static void StartTcpListener()
    {
        int port = 12345;
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);

        try
        {
            tcpListener.Start();
            Console.WriteLine("TCP listener started on port " + port);

            while (true)
            {
                Console.WriteLine("Waiting for a TCP connection...");
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("TCP client connected!");

                Task.Run(() => HandleTcpClient(tcpClient));
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("TCP SocketException: " + e);
        }
        finally
        {
            tcpListener.Stop();
        }
    }

    static void HandleTcpClient(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received TCP: " + data);

                byte[] response = Encoding.UTF8.GetBytes("TCP Data received");
                stream.Write(response, 0, response.Length);
                Console.WriteLine("Sent TCP: TCP Data received");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("TCP Client exception: " + e);
        }
        finally
        {
            tcpClient.Close();
        }
    }

    static void StartUdpListener()
    {
        int port = 12345;
        UdpClient udpClient = new UdpClient(port);

        try
        {
            Console.WriteLine("UDP listener started on port " + port);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine("Received UDP from {0}: {1}", remoteEndPoint.ToString(), message);

                byte[] response = Encoding.UTF8.GetBytes("UDP Data received");
                udpClient.Send(response, response.Length, remoteEndPoint);
                Console.WriteLine("Sent UDP: UDP Data received");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("UDP SocketException: " + e);
        }
        finally
        {
            udpClient.Close();
        }
    }
}
