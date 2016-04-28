using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupervisor.Code.Classes
{
    //public class AccessTokenInfo
    //{
    //    /// <summary>
    //    /// access_token
    //    /// </summary>
    //    public string access_token { get; set; }

    //    /// <summary>
    //    /// 凭证有效时间，单位：秒 
    //    /// </summary>
    //    public long expires_in { get; set; }

    //    /// <summary>
    //    /// 获取时间
    //    /// </summary>
    //    public DateTime GetTime { get; set; }
    //}
    ///// <summary>
    ///// 获取企业登陆access_token
    ///// </summary>
    //public static class BLLAccessToken
    //{
    //    static AccessTokenInfo TokenInfo = null;
    //    public static string GetAccessToken()
    //    {
    //        string AccessToken = "";
    //        DateTime now = DateTime.Now;
    //        if (TokenInfo == null)  //首次获取
    //        {
    //            TokenInfo = UpDateAccessToken();
    //        }
    //        else
    //        {
    //            if (TokenInfo.GetTime.AddSeconds(TokenInfo.expires_in - 30) < now) //提前30秒重新获取
    //            {
    //                TokenInfo = UpDateAccessToken();
    //            }
    //        }
    //        AccessToken = TokenInfo.access_token;
    //        return AccessToken;
    //    }
    //    private static AccessTokenInfo UpDateAccessToken()
    //    {
    //        string CorpId = AppIdInfo.GetCorpId();//corpid
    //        string Secret = AppIdInfo.GetSecret(); //corpsecret
    //        AccessTokenInfo info = new AccessTokenInfo();
    //        WebUtils ut = new WebUtils();
    //        /// https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=wxb7f1db8fd6aa9d68&corpsecret=aoxZ7D5-SgLRUbKY2fwQykW36RqxoIdNIn1pIiGy9iSdXgMHwQCzUsniQVAsBCTt
    //        string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
    //        var url = string.Format(urlFormat, CorpId, Secret);
    //        string temp = ut.DoGet(url);
    //        try
    //        {
    //            AccessTokenInfo tempAccessTokenjson = Tools.JsonStringToObj<AccessTokenInfo>(temp);
    //            info.access_token = tempAccessTokenjson.access_token;
    //            info.expires_in = tempAccessTokenjson.expires_in;
    //            info.GetTime = DateTime.Now;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        return info;
    //    }

    //}
    //public class MsgBase
    //{
    //    public MsgBase()
    //    {
    //        this.safe = "0"; //表示是否是保密消息，0表示否，1表示是，默认0
    //    }
    //    /// <summary>
    //    /// UserID列表（消息接收者，多个接收者用‘|’分隔）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送
    //    /// </summary>
    //    public string touser { get; set; }

    //    /// <summary>
    //    /// PartyID列表，多个接受者用‘|’分隔。当touser为@all时忽略本参数
    //    /// </summary>
    //    public string toparty { get; set; }

    //    /// <summary>
    //    /// TagID列表，多个接受者用‘|’分隔。当touser为@all时忽略本参数
    //    /// </summary>
    //    public string totag { get; set; }
    //    /// <summary>
    //    /// 消息类型
    //    /// </summary>
    //    public string msgtype { get; set; }
    //    /// <summary>
    //    /// 企业应用的id，整型。可在应用的设置页面查看
    //    /// </summary>
    //    public int agentid { get; set; }

    //    /// <summary>
    //    /// 表示是否是保密消息，0表示否，1表示是，默认0
    //    /// </summary>     
    //    public string safe { get; set; }
    //}
    //public static class MsgType
    //{
    //    public enum MsgBaseEnum
    //    {
    //        Text = 1,
    //        image = 2,
    //        voice = 3,
    //        video = 4,
    //        file = 5,
    //        news = 6,
    //        mpnews = 7

    //    };
    //    public static string GetMsgTypeText(MsgBaseEnum type)
    //    {
    //        string text = "";
    //        switch (type)
    //        {
    //            case MsgBaseEnum.Text:
    //                text = "text";
    //                break;
    //            case MsgBaseEnum.image:
    //                text = "image";
    //                break;
    //            case MsgBaseEnum.voice:
    //                text = "voice";
    //                break;
    //            case MsgBaseEnum.video:
    //                text = "video";
    //                break;
    //            case MsgBaseEnum.file:
    //                text = "file";
    //                break;
    //            case MsgBaseEnum.news:
    //                text = "news";
    //                break;
    //            case MsgBaseEnum.mpnews:
    //                text = "mpnews";
    //                break;
    //            default:
    //                throw new Exception("type=" + type + "，此类型的消息没有实现");

    //        }
    //        return text;
    //    }
    //}
    //public class TextMsg : MsgBase
    //{
    //    public TextMsg(string content)
    //    {
    //        this.text = new TextMsgContent(content);
    //        this.msgtype = MsgType.GetMsgTypeText(MsgType.MsgBaseEnum.Text);
    //    }
    //    public TextMsgContent text { get; set; }
    //}
    //public class TextMsgContent
    //{
    //    public TextMsgContent(string content)
    //    {
    //        this.content = content;
    //    }
    //    public string content { get; set; }
    //}
    //public static class BLLMsg
    //{

    //    public static bool SendMessage(MsgBase data)
    //    {
    //        string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
    //        string accessToken = BLLAccessToken.GetAccessToken();
    //        var url = string.Format(urlFormat, accessToken);
    //        //WebUtils ut = new WebUtils();
    //        //var postData = Tools.ToJsonString<MsgBase>(data);
    //        //数据不用加密发送  
    //        //LogInfo.Info("发送消息: " + postData);
    //        //string sendResult = ut.DoPost(url, postData);
    //       // SendMsgResult tempAccessTokenjson = Tools.JsonStringToObj<SendMsgResult>(sendResult);
    //        //if (tempAccessTokenjson.HasError())
    //        //{
    //        //    //LogInfo.Error("发送消息错误: " + Tools.ToJsonString<SendMsgResult>(tempAccessTokenjson));

    //        //    return false;
    //        //}


    //        return true;
    //    }
    //}
}
