using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class EventData
{
    public EventInfo Event { get; set; }
}

public class EventInfo
{
    public string Name { get; set; }
    public long Time { get; set; }
    public string Location { get; set; }
    public ConfidenceData Confidence { get; set; }
}

public class ConfidenceData
{
    [JsonExtensionData]
    private Dictionary<string, JToken>? _data;

    public Dictionary<string, Dictionary<string, int>>? Name
    {
        get
        {
            if (_data != null && _data.TryGetValue("name", out var value) && value is JObject jobject)
            {
                var dictionary = jobject.ToObject<Dictionary<string, int>>();
                return dictionary != null ? new Dictionary<string, Dictionary<string, int>> { { "name", dictionary } } : null;
            }
            return null;
        }
    }

    public Dictionary<string, Dictionary<string, int>>? Location
    {
        get
        {
            if (_data != null && _data.TryGetValue("location", out var value) && value is JObject jobject)
            {
                var dictionary = jobject.ToObject<Dictionary<string, int>>();
                return dictionary != null ? new Dictionary<string, Dictionary<string, int>> { { "location", dictionary } } : null;
            }
            return null;
        }
    }

    public Dictionary<string, Dictionary<string, int>>? Time
    {
        get
        {
            if (_data != null && _data.TryGetValue("time", out var value) && value is JObject jobject)
            {
                var dictionary = jobject.ToObject<Dictionary<string, int>>();
                return dictionary != null ? new Dictionary<string, Dictionary<string, int>> { { "time", dictionary } } : null;
            }
            return null;
        }
    }
}
