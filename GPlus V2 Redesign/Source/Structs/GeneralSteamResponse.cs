using GPlus.Source.Enums;
namespace GPlus.Source.Structs
{
    public struct GeneralSteamResponse
    {
        public string? Data;
        public int? Progress;
        public ResponseType responseType;
        public ClientResponse response;
    }
}
