using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tiamet2._0.Data;

namespace Tiamet2._0.Commands
{
    public static class Diablo4
    {
        public static async Task<bool> CheckEvents(string url, DiscordSocketClient client)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string eventJsonData = await httpClient.GetStringAsync(url);

                    EventData eventData = JsonConvert.DeserializeObject<EventData>(eventJsonData);
                    var botChannel = client.GetChannel(1123379017656053861) as SocketTextChannel;
                    Console.WriteLine($"{url} Checked!");
                    // Access the deserialized data and perform actions
                    if (eventData.Event.Name != null)
                    {
                        await botChannel.SendMessageAsync($"{eventData.Event.Name} at {eventData.Event.Location} will start in: <t:{eventData.Event.Time / 1000}:R>");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Events Check Error: {ex.Message}");
                return false;
            }
        }

    }
}
