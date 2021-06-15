using Newtonsoft.Json;

namespace BifrostRemoteDesktop.Common.Models.Commands
{

    public abstract class RemoteControlCommandArgs: IRemoteControlCommandArgs
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
}
