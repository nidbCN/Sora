using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sora.ApiMessageParse;
using Sora.CQCodes;
using Sora.Enumeration.ApiEnum;
using Sora.EventArgs.OnebotEvent.MessageEvent;
using Sora.Module;
using Sora.Module.Info;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 私聊消息事件参数
    /// </summary>
    public sealed class PrivateMessageEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 消息内容
        /// </summary>
        public Message Message { get; private set; }

        /// <summary>
        /// 消息发送者实例
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 发送者信息
        /// </summary>
        public PrivateSenderInfo SenderInfo { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="serverPrivateMsg">服务器消息事件参数</param>
        internal PrivateMessageEventArgs(Guid connectionGuid, string eventName, ApiPrivateMsgEventArgs serverPrivateMsg)
            : base(connectionGuid, eventName, serverPrivateMsg.SelfID, serverPrivateMsg.Time)
        {
            this.Message = new Message(connectionGuid, serverPrivateMsg.MessageId, serverPrivateMsg.RawMessage,
                                       MessageParse.ParseMessageList(serverPrivateMsg.MessageList),
                                       serverPrivateMsg.Time, serverPrivateMsg.Font);
            this.Sender     = new User(connectionGuid, serverPrivateMsg.UserId);
            this.SenderInfo = serverPrivateMsg.SenderInfo;
        }
        #endregion

        #region 快捷方法
        /// <summary>
        /// 快速回复
        /// </summary>
        /// <param name="message">
        /// <para>消息</para>
        /// <para>可以为<see cref="string"/>/<see cref="CQCode"/>/<see cref="List{T}"/>(T = <see cref="CQCode"/>)</para>
        /// <para>其他类型的消息会被强制转换为纯文本</para>
        /// </param>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="messageId"/> 发送消息的id</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, int messageId)> Reply(params object[] message)
        {
            return await base.SoraApi.SendPrivateMessage(this.Sender.Id, message);
        }

        /// <summary>
        /// 没什么用的复读功能
        /// </summary>
        /// <returns>
        /// <para><see cref="APIStatusType"/> API执行状态</para>
        /// <para><see langword="messageId"/> 发送消息的id</para>
        /// </returns>
        public async ValueTask<(APIStatusType apiStatus, int messageId)> Repeat()
        {
            return await base.SoraApi.SendPrivateMessage(this.Sender.Id, this.Message.MessageList);
        }
        #endregion
    }
}
