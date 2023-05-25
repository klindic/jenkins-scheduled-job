namespace jenkins_scheduled_job_test.Utils
{
    using Newtonsoft.Json;

    internal class EmailConfig
    {
        [JsonProperty("SendGrid:ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("SendGrid:FromEmailAddress")]
        public string FromEmailAddress { get; set; }

        [JsonProperty("SendGrid:FromName")]
        public string FromName { get; set; }
    }
}
