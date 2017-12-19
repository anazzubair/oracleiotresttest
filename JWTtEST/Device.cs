namespace JWTtEST
{
    public class Device
    {
        public long Id { get; set; }
        public string ActivationId { get; set; }
        public string ActivationSecret { get; set; }
        public string  DeviceEndpointId { get; set; }
        public string RSAKeyXML { get; set; }
        public virtual Registration Registration { get; set; }
    }
}