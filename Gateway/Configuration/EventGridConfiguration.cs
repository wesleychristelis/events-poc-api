namespace EventualityPOCApi.Gateway.Configuration
{
    public class EventGridConfiguration
    {
        public bool Enabled { get; set; }
        public string PersonProfileContextPerceptionTopicKey { get; set; }
        public string PersonProfileContextPerceptionTopicUrl { get; set; } 
    }
}
