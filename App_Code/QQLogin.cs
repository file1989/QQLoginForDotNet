using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Web.SessionState;

/// <summary>
/// QQ信息
/// </summary>
public class QQ
{
    public string ret = string.Empty;/*值为0，表示正确返回*/
    public string msg = string.Empty;
    public string is_lost = string.Empty;
    public string nickname = string.Empty;
    public string gender = string.Empty;
    public string province = string.Empty;
    public string city = string.Empty;
    public string year = string.Empty;
    public string figureurl = string.Empty;
    public string figureurl_1 = string.Empty;
    public string figureurl_2 = string.Empty;
    public string figureurl_qq_1 = string.Empty;
    public string figureurl_qq_2 = string.Empty;
    public string is_yellow_vip = string.Empty;
    public string vip = string.Empty;
    public string yellow_vip_level = string.Empty;
    public string level = string.Empty;
    public string is_yellow_year_vip = string.Empty;
    /*QQ信息
         {
            "ret": 0, 
            "msg": "", 
            "is_lost": 0, 
            "nickname": "测试QQ", 
            "gender": "男", 
            "province": "广东", 
            "city": "广州", 
            "year": "1998", 
            "figureurl": "http://qzapp.qlogo.cn/qzapp/101354702/1B3199AEA884C03571E6C265ABE021C9/30", 
            "figureurl_1": "http://qzapp.qlogo.cn/qzapp/101354702/1B3199AEA884C03571E6C265ABE021C9/50", 
            "figureurl_2": "http://qzapp.qlogo.cn/qzapp/101354702/1B3199AEA884C03571E6C265ABE021C9/100", 
            "figureurl_qq_1": "http://q.qlogo.cn/qqapp/101354702/1B3199AEA884C03571E6C265ABE021C9/40", 
            "figureurl_qq_2": "http://q.qlogo.cn/qqapp/101354702/1B3199AEA884C03571E6C265ABE021C9/100", 
            "is_yellow_vip": "0", 
            "vip": "0", 
            "yellow_vip_level": "0", 
            "level": "0", 
            "is_yellow_year_vip": "0"
        }
        
    */
}

/// <summary>
/// QQ登录数据
/// </summary>
public class QQLoginData {
    public string openid = string.Empty;/*唯一值，与QQ号对应*/
    public string access_token = string.Empty;
    /// <summary>
    /// QQ信息
    /// </summary>
    public QQ QQ = new QQ();
    /// <summary>
    /// QQ头像
    /// </summary>
    public Image QQImage {
        get {
            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(QQ.figureurl_qq_1);
                System.Net.WebResponse webres = webreq.GetResponse();
                Stream stream = webres.GetResponseStream();
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                stream.Close();
                return image;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}

/// <summary>
/// QQ登录
/// </summary>
public class QQLogin
{
    /// <summary>
    /// 从url中提取参数值
    /// </summary>
    /// <param name="urlString">URL字符串</param>
    /// <param name="key">参数名称</param>
    /// <returns></returns>
    private static string getUrlParam(string urlString, string key)
    {
        Regex reg = new Regex("[?&]*" + key + "=([^&]+)(&|$)");
        Match m = reg.Match(urlString);
        return (m.Success ? m.Groups[1].Value : string.Empty);
    }
    /// <summary>
    /// 根据url地址，以GET 形式获取数据
    /// </summary>
    /// <param name="Url"></param>
    /// <returns></returns>
    public static string GetData(string Url)
    {
        try
        {
            WebRequest hr = WebRequest.Create(Url);
            hr.Method = "GET";

            System.Net.WebResponse response = hr.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string ReturnVal = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return ReturnVal;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 获取访问令牌
    /// </summary>
    /// <param name="client_id">申请QQ登录成功后，分配给网站的appid。</param>
    /// <param name="client_secret">申请QQ登录成功后，分配给网站的appkey。</param>
    /// <param name="code">如果用户成功登录并授权，则会跳转到指定的回调地址，并在URL中带上Authorization Code。</param>
    /// <param name="redirect_uri">成功授权后的回调地址，必须是注册appid时填写的主域名下的地址，建议设置为网站首页或网站的用户中心。注意需要将url进行URLEncode。</param>
    /// <returns></returns>
    public static string GetAccessToken(string client_id, string client_secret, string code, string redirect_uri)
    {
        //错误返回值
        //callback( {"error":100019,"error_description":"code to access token error"} ); 
        //正常返回值
        //access_token=82CEF5B9CB5BAC3D7B4A2E01103B217D&expires_in=7776000&refresh_token=EDA4B34F7279C243492945B8C6401981 
        string result = GetData(string.Format("https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}", client_id, client_secret, code, redirect_uri));
        //错误处理
        if (result.StartsWith("callback(") && result.EndsWith(");"))
        {
            result = result.Replace("callback(", "").Replace(");", "");
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, string> res = jss.Deserialize<Dictionary<string, string>>(result);
            throw new Exception(string.Format("error:{0},error_description:{1}", res["error"], res["error_description"]));
        }
        return getUrlParam(result, "access_token");

    }
    /// <summary>
    /// 获取用户的ID，与QQ号码一一对应。 
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns></returns>
    public static string GetOpenId(string accessToken)
    {
        //正常返回值
        //callback( {"client_id":"101354702","openid":"BC5F9230E1D2F3C09C891E2F043BF489"} ); 
        //错误返回值
        //callback( {"error":100016,"error_description":"access token check failed"} );
        string result = GetData(string.Format("https://graph.qq.com/oauth2.0/me?access_token={0}", accessToken));

        result = result.Replace("callback(", "").Replace(");", "");
        System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
        Dictionary<string, string> res = jss.Deserialize<Dictionary<string, string>>(result);
        //错误处理
        if (res.ContainsKey("error"))
        {
            throw new Exception(string.Format("error:{0},error_description:{1}", res["error"], res["error_description"]));
        }
        return res["openid"];
    }
    /// <summary>
    /// 获取QQ用户信息
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="oauth_consumer_key">申请QQ登录成功后，分配给应用的appid</param>
    /// <param name="openId">用户的ID，与QQ号码一一对应。 </param>
    /// <returns></returns>
    public static QQ GetQQUserInfo(string accessToken, string oauth_consumer_key, string openId)
    {
        //正常返回值
        //{ "ret": 0, "msg": "", "is_lost":0, "nickname": "智瓶子", "gender": "男", "province": "广东", "city": "广州", "year": "1989", "figureurl": "http:\/\/qzapp.qlogo.cn\/qzapp\/101354702\/BC5F9230E1D2F3C09C891E2F043BF489\/30", "figureurl_1": "http:\/\/qzapp.qlogo.cn\/qzapp\/101354702\/BC5F9230E1D2F3C09C891E2F043BF489\/50", "figureurl_2": "http:\/\/qzapp.qlogo.cn\/qzapp\/101354702\/BC5F9230E1D2F3C09C891E2F043BF489\/100", "figureurl_qq_1": "http:\/\/q.qlogo.cn\/qqapp\/101354702\/BC5F9230E1D2F3C09C891E2F043BF489\/40", "figureurl_qq_2": "http:\/\/q.qlogo.cn\/qqapp\/101354702\/BC5F9230E1D2F3C09C891E2F043BF489\/100", "is_yellow_vip": "0", "vip": "0", "yellow_vip_level": "0", "level": "0", "is_yellow_year_vip": "0" } 
        //错误返回值
        //{"ret":-23,"msg":"token is invalid"} 或{"ret":-22,"msg":"openid is invalid"}
        string result = GetData(string.Format("https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}", accessToken, oauth_consumer_key, openId));
        System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
        Dictionary<string, string> ret = jss.Deserialize<Dictionary<string, string>>(result);
        // 错误处理
        if (ret["ret"] != "0")
        {
            throw new Exception(string.Format("ret:{0},msg:{1}", ret["ret"], ret["msg"]));
        }
        return jss.Deserialize<QQ>(result);
    }
    /// <summary>
    /// 获取登录QQ信息
    /// </summary>
    /// <param name="client_id">申请QQ登录成功后，分配给网站的appid。</param>
    /// <param name="client_secret">申请QQ登录成功后，分配给网站的appkey。</param>
    /// <param name="redirect_uri">回调地址</param>
    /// <returns></returns>
    public static QQLoginData GetQQLoginData(string client_id, string client_secret, string redirect_uri)
    {
        QQLoginData qqLoginData = new QQLoginData();
        string code = HttpContext.Current.Request["code"] ?? string.Empty;
        /*获取Access Token*/
        qqLoginData.access_token = GetAccessToken(client_id, client_secret, code, redirect_uri);
        
        /*获取到用户OpenID*/
        qqLoginData.openid = GetOpenId(qqLoginData.access_token);
        
        /*获取QQ用户信息*/
        qqLoginData.QQ = GetQQUserInfo(qqLoginData.access_token,client_id, qqLoginData.openid);
        
        //HttpContext.Current.Session.Add("QQLogin", qqLoginData);
        return qqLoginData;
    }
    /// <summary>
    /// 打开QQ登录链接
    /// </summary>
    /// <param name="client_id">申请QQ登录成功后，分配给网站的appid。</param>
    /// <param name="redirect_uri">成功授权后的回调地址，必须是注册appid时填写的主域名下的地址，建议设置为网站首页或网站的用户中心。注意需要将url进行URLEncode。</param>
    /// <param name="state">client端的状态值。用于第三方应用防止CSRF攻击，成功授权后回调时会原样带回。请务必严格按照流程检查用户与state参数状态的绑定。</param>
    public static void Login(string client_id, string redirect_uri, string state)
    {
        HttpContext.Current.Response.Redirect(string.Format("https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}", client_id, redirect_uri, state), true);
    }


    
}