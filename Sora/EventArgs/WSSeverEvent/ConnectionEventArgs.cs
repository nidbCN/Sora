using Fleck;

namespace Sora.EventArgs.WSSeverEvent
{
    public sealed class ConnectionEventArgs : BaseEventArgs
    {
        #region 属性
        /// <summary>
        /// 客户端类型
        /// </summary>
        public string Role { get; set; }
        #endregion

        #region 构造函数

        public ConnectionEventArgs(string role, IWebSocketConnectionInfo connectionInfo)
        {
            
            this.Role           = role;
            base.ConnectionInfo = connectionInfo;
        }
        #endregion
    }
}