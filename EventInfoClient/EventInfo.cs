using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace EventInfoClient
{
    public enum EventType
    {
        Concert,
        Exhibition,
        Sport,
        Theater,
        Other
    }

    public class EventInfo
    {
        public EventInfo() { }

        public EventInfo(string name, string description, EventType type, DateTime date)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.date = date;
        }

        public EventInfo(long id, string name, string description, EventType type, DateTime date) 
            : this(name, description, type, date)
        {
            this.id = id;
        }

        public long id { get; set; }
        public string name { get; set ; }
        public string description { get; set; }
        public EventType type { get; set; }
        public DateTime date { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new CustomDateTimeConverter(), new StringEnumConverter());
        }
    }
}
