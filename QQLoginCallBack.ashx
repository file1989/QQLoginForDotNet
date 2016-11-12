<%@ WebHandler Language="C#" Class="QQLoginCallBack" %>

using System;
using System.Web;
using System.Reflection;

public class QQLoginCallBack : IHttpHandler
{
    public bool IsReusable { get { return false; } }
    public void ProcessRequest(HttpContext context) {
        context.Response.ContentType = "text/plain";
        string method = context.Request["method"] ?? string.Empty;
        if (method == "")
        {
            //QQLoginData LoginData = QQLogin.GetQQLoginData("申请QQ登录成功后，分配给网站的appid。", "申请QQ登录成功后，分配给网站的appkey。", "回调地址");
            //context.Response.Write(LoginData.QQ.nickname);
        }
        else {
            MethodInfo mi = this.GetType().GetMethod(method);
            mi.Invoke(this, new object[] { context });
        }
        
    }
    public void Login(HttpContext context)
    {
        //QQLogin.Login("申请QQ登录成功后，分配给网站的appid。", "成功授权后的回调地址", "client端的状态值。");
    }

}