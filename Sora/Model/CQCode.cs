using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Sora.Enumeration;
using Sora.Model.CQCodeModel;
using Sora.Model.Message;

namespace Sora.Model
{
    public class CQCode
    {
        #region 字段
        private static readonly List<Regex> FileRegices = new List<Regex>
        {
            new Regex(@"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*\.[\w]+$", RegexOptions.Compiled), //绝对路径
            new Regex(@"^base64:\/\/[\/]?([\da-zA-Z]+[\/+]+)*[\da-zA-Z]+([+=]{1,2}|[\/])?$", RegexOptions.Compiled),//base64
            new Regex(@"^(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$", RegexOptions.Compiled),//网络图片链接
            new Regex(@"^[\w,\s-]+\.[\w]+$", RegexOptions.Compiled)
        };
        #endregion

        #region 属性
        /// <summary>
        /// CQ码类型
        /// </summary>
        public CQFunction Function { get; internal set; }

        /// <summary>
        /// CQ码数据实例
        /// </summary>
        internal object CQData { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造CQ码实例
        /// </summary>
        /// <param name="cqFunction">CQ码类型</param>
        /// <param name="dataObj"></param>
        internal CQCode(CQFunction cqFunction, object dataObj)
        {
            this.Function = cqFunction;
            this.CQData   = dataObj;
        }
        #endregion

        #region CQ码构建方法
        /// <summary>
        /// 纯文本
        /// </summary>
        /// <param name="msg">文本消息</param>
        public static CQCode CQText(string msg)
        {
            return new CQCode(CQFunction.Text,
                              new Text {Context = msg});
        }

        /// <summary>
        /// At CQ码
        /// </summary>
        /// <param name="uid">用户uid</param>
        public static CQCode CQAt(long uid)
        {
            //TODO 账号最小值判断
            return new CQCode(CQFunction.At,
                              new At {Traget = uid.ToString()});
        }

        /// <summary>
        /// At全体 CQ码
        /// </summary>
        public static CQCode CQAtAll()
        {
            return new CQCode(CQFunction.At,
                              new At {Traget = "all"});
        }

        /// <summary>
        /// 表情CQ码
        /// </summary>
        /// <param name="id">表情 ID</param>
        public static CQCode CQFace(int id)
        {
            return new CQCode(CQFunction.Face,
                              new Face {Id = id});
        }

        /// <summary>
        /// 语音CQ码
        /// </summary>
        /// <param name="data">文件名/绝对路径/URL/base64</param>
        /// <param name="isMagic">是否为变声</param>
        /// <param name="useCache">是否使用已缓存的文件</param>
        /// <param name="useProxy">是否通过代理下载文件</param>
        /// <param name="timeout">超时时间，默认为<see langword="null"/>(不超时)</param>
        public static CQCode CQRecord(string data, bool isMagic = false, bool useCache = true, bool useProxy = true,
                                      int? timeout = null)
        {
            return new CQCode(CQFunction.Record,
                              new Record
                              {
                                  RecordFile = ParseDataStr(data),
                                  Magic      = isMagic ? 1 : 0,
                                  Cache      = useCache ? 1 : 0,
                                  Proxy      = useProxy ? 1 : 0,
                                  Timeout    = timeout
                              });
        }

        /// <summary>
        /// 图片CQ码
        /// </summary>
        /// <param name="data">图片名/绝对路径/URL/base64</param>
        /// <param name="isFlash">是否为闪照，默认为<see langword="false"/></param>
        /// <param name="useCache">是否使用已缓存的文件</param>
        /// <param name="useProxy">是否通过代理下载文件</param>
        /// <param name="timeout">超时时间，默认为<see langword="null"/>(不超时)</param>
        public static CQCode CQImage(string data, bool isFlash = false, bool useCache = true, bool useProxy = true,
                                     int? timeout = null)
        {
            return new CQCode(CQFunction.Image,
                              new Image
                              {
                                  ImgFile = ParseDataStr(data),
                                  ImgType = isFlash ? "flash" : string.Empty,
                                  Cache   = useCache ? 1 : 0,
                                  Proxy   = useProxy ? 1 : 0,
                                  Timeout = timeout
                              });
        }
        #endregion

        #region 获取CQ码内容(仅用于序列化)
        public OnebotMessage ToOnebotMessage() => new OnebotMessage
        {
            MsgType = this.Function,
            RawData = JObject.FromObject(this.CQData)
        };
        #endregion

        #region 私有方法
        /// <summary>
        /// 处理传入数据
        /// </summary>
        /// <param name="dataStr">数据</param>
        private static string ParseDataStr(string dataStr)
        {
            bool isMatch = false;
            foreach (Regex regex in FileRegices)
            {
                isMatch |= regex.IsMatch(dataStr);
            }
            //判断是否是文件名
            if (FileRegices[0].IsMatch(dataStr))
            {
                return $"file:///{dataStr}";
            }
            if (!isMatch) throw new NotSupportedException("not supported image data");
            return dataStr;
        }
        #endregion
    }
}
