namespace BifrostRemoteDesktop.Common.Network
{
    public static class TransmissionContext
    {
        public const int INPUT_TCP_PORT = 3978;
        public const int RECEIVER_BUFFER_SIZE = 1024;

        public const char START_OF_TEXT_CHAR = '\x02';
        public const char END_OF_TEXT_CHAR = '\x03';
        public const char TEXT_SEGMENTATION_CHAR = ';';
    }
}
