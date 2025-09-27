using GPlus.Source.Enums;
using System.Text.Json;

namespace GPlus.Source.Structs
{
    internal class GMODRequest
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public GMODRequestTypes Type { get; set; }
        public bool ExpectResponse { get; set; }
        public string[] Args { get; set; } = Array.Empty<string>();

        public string ToJson() => JsonSerializer.Serialize(this);

        public static GMODRequest FromJson(string json) =>
            JsonSerializer.Deserialize<GMODRequest>(json)!;
    }

    internal class GMODResponse
    {
        public Guid RequestId { get; set; }
        public string Result { get; set; }
    }
}
