﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class WinRtApplicationOnlyAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        public string BasicToken { get; set; }
        public string BearerToken { get; set; }
        public string OAuth2Token { get; set; }
        public string OAuth2InvalidateToken { get; set; }

        public WinRtApplicationOnlyAuthorizer()
        {
            OAuth2Token = "https://api.twitter.com/oauth2/token";
            OAuth2InvalidateToken = "https://api.twitter.com/oauth2/invalidate_token";
        }

        public void Authorize()
        {
            throw new NotImplementedException("Please call AuthorizeAsync instead.");
        }

        public async Task AuthorizeAsync()
        {
            EncodeCredentials();
            await GetBearerTokenAsync();
        }

        public override bool IsAuthorized
        {
            get
            {
                return !string.IsNullOrEmpty(BearerToken);
            }
        }

        public async Task InvalidateAsync()
        {
            EncodeCredentials();

            var client = new HttpClient();
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, OAuth2InvalidateToken);
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", "LINQ to Twitter v2.1");
            req.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            req.Headers.ExpectContinue = false;

            var msg = await client.SendAsync(req);

            string response = await msg.Content.ReadAsStringAsync();

            var responseJson = JsonMapper.ToObject(response);
            BearerToken = responseJson.GetValue<string>("access_token");
        }
  
        async Task GetBearerTokenAsync()
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, OAuth2Token);
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", "LINQ to Twitter v2.1");
            req.Headers.ExpectContinue = false;
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var msg = await client.SendAsync(req);

            string response = await msg.Content.ReadAsStringAsync();

            var responseJson = JsonMapper.ToObject(response);
            BearerToken = responseJson.GetValue<string>("access_token");
        }

        internal void EncodeCredentials()
        {
            string encodedConsumerKey = BuildUrlHelper.UrlEncode(Credentials.ConsumerKey);
            string encodedConsumerSecret = BuildUrlHelper.UrlEncode(Credentials.ConsumerSecret);

            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;

            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);

            string base64Credentials = Convert.ToBase64String(credBytes);

            BasicToken = base64Credentials;
        }


        /// <summary>
        /// OAuth Get
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>Request to be sent to Twitter</returns>
        public new WebRequest Get(Request request)
        {
            var req = WebRequest.Create(request.FullUrl) as HttpWebRequest;
            if (req != null)
            {
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken;

                InitializeRequest(req);
            }

            return req;
        }

        /// <summary>
        /// OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>request to send</returns>
        public override HttpWebRequest PostRequest(Request request, IDictionary<string, string> postData)
        {
            var req = WebRequest.Create(request.FullUrl) as HttpWebRequest;

            if (req != null)
            {
                req.Method = HttpMethod.POST.ToString();
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken;

                InitializeRequest(req);
            }

            return req;
        }

        /// <summary>
        /// OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>request to send</returns>
        public override HttpWebResponse Post(Request request, IDictionary<string, string> postData)
        {
            var req = PostRequest(request, postData);
            return Utilities.AsyncGetResponse(req);
        }

        /// <summary>
        /// Async OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>HttpWebRequest for post</returns>
        public override HttpWebRequest PostAsync(Request request, IDictionary<string, string> postData)
        {
            var req = WebRequest.Create(
                    ProxyUrl + request.Endpoint +
                    (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                    request.QueryString)
                as HttpWebRequest;

            if (req != null)
            {
                req.Method = HttpMethod.POST.ToString();
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken;

                InitializeRequest(req);
            }

            return req;
        }
    }
}
