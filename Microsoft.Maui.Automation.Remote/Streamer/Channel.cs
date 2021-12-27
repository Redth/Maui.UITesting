using System.IO;

namespace Streamer
{
    public class Channel
    {
        public static ClientChannel CreateClient(Stream stream)
            => new ClientChannel(stream);

        public static ServerChannel CreateServer()
            => new ServerChannel();
    }
}