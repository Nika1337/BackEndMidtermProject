using System.Net;
using System.Net.Sockets;

var portNumber = 8080;
var listener = new TcpListener(IPAddress.Any, portNumber);
listener.Start();
Console.WriteLine($"Listening for request started on port {portNumber}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Accepted new client");
    client.Close();
}
