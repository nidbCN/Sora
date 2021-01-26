using Newtonsoft.Json;

namespace Sora.Entities.Info
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public struct ClientInfo
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        [JsonProperty(PropertyName = "app_id")]
        public long AppId { get; internal set; }

        /// <summary>
        /// 设备名
        /// </summary>
        [JsonProperty(PropertyName = "device_name")]
        public string DeviceName { get; internal set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [JsonProperty(PropertyName = "device_kind")]
        public string Device { get; internal set; }
    }
}