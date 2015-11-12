using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Accuracy.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

#pragma warning disable 1591
namespace Accuracy.Web.Common
{
    public class Util
    {
        public static HttpRequestMessage Request
        {
            get { return (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"]; }
        }
         
        public static HttpResponseMessage Ok<T>(T content)
        {
            return Request.CreateResponse(HttpStatusCode.OK, content);
        }


        public static HttpResponseMessage Fail(string errorCode, string message)
        {
            HttpError err = new HttpError(message);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NotFound, err);
            response.Headers.Add("Error-Code", errorCode);
            return response;
        }

        public static void AddHeaderCode(string code)
        {
            HttpContext.Current.Response.AppendHeader("Error-Code", code);
        }

        public static T GetFirstHeaderValueOrDefault<T>(HttpRequestMessage Request, string headerKey,
   Func<HttpRequestMessage, string> defaultValue,
   Func<string, T> valueTransform)
        {
            IEnumerable<string> headerValues;
            HttpRequestMessage message = Request ?? new HttpRequestMessage();
            if (!message.Headers.TryGetValues(headerKey, out headerValues))
                return valueTransform(defaultValue(message));
            string firstHeaderValue = headerValues.FirstOrDefault() ?? defaultValue(message);
            return valueTransform(firstHeaderValue);
        }

       

        public static async Task<string> GenIdSync()
        {
            return await Task.Run(() => Helper.GenerateRandomId());
        }


        public static HttpResponseMessage ReturnError(HttpRequestMessage Request, string Code, string Message)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError, Message);
            response.Headers.Add("Error-Code", Code);
            throw new HttpResponseException(response);
        }


     


        public static JObject GenerateLocalAccessTokenResponse(string userName, string studentId, string avatar, string universityId)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            
            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("studentId", studentId),
                                        new JProperty("avatar", avatar),
                                        new JProperty("universityId", universityId),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        public static JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                       
            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        public static string ToString(string arg)
        {
            return arg.ToString();
        }

       

    }

}