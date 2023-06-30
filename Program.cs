using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tiamet2._0;
using Tiamet2._0.Data;

class Program
{
    static void Main(string[] args)
        => new Tiamat().RunAsync().GetAwaiter().GetResult();
}