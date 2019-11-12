using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace B2B.OAuth
{
    public class OAuthClass
    {
        public const string clientId = "e0e06d349106b32a94a0d3d056e316eefa7a3020590e19ca6de0b6dafdd3e07c";
        public const string secretId = "50dcd41d0130c415885a3c8c10f586ef314c04cf08d3eb7de974e457b1853306";
        public const string callbackUrls = "urn:ietf:wg:oauth:2.0:oob";
        public const string GetTokenApiUrl = "https://api.parasut.com/oauth/token?client_id=" + clientId + "&client_secret=" + secretId + "&username=ismetsinar@fortechteknoloji.com&password=dctb24&grant_type=password&redirect_uri=" + callbackUrls;
        public const string ApiRefreshUrl = "https://api.parasut.com/oauth/token";
        public const string ApiUrl = "https://api.parasut.com/";
    }
    public class OAuthSonucSinifi
    {
        public string clientId { get; set; }
        public string secretId { get; set; }
        public string callbackId { get; set; }
        public Uri ApiUrl { get; set; }
        public string ApiRefreshUrl { get; set; }

        public OAuthSonucSinifi _getOauthProperties()
        {
            OAuthSonucSinifi _o = new OAuthSonucSinifi();
            try
            {
                _o.clientId = OAuthClass.clientId;
                _o.secretId = OAuthClass.secretId;
                _o.callbackId = OAuthClass.callbackUrls;
                _o.ApiUrl = new Uri(OAuthClass.GetTokenApiUrl);
                _o.ApiRefreshUrl = OAuthClass.ApiRefreshUrl;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _o;
        }
    }
    public class OAuthClass_GetAccessTokenClass
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    public class ApiRequests
    {
        
    }


}