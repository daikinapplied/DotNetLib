namespace Daikin.DotNetLib.Serial.Com
{
    public struct ComEventArgs
    {
        public uint Length { get; set; }

        public byte[] Message { get; set; }

        public int Result { get; set; }

        public string ResultDescription { get; set; }

        public override string ToString()
        {
            return Conversion.BytesToAsciiHex(Message);
        }
    }
}
