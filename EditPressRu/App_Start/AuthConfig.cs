using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditPressRu.Models.DB;

namespace EditPressRu
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // следует обновить сайт. Дополнительные сведения: c

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "bmTDp6eziULvmXjWAlEPTQ3MF",
            //    consumerSecret: "S0ZlayQ2vqdy7lxCwWhRGvgpVdLoMYJ5p4T93OXrW7KkD3A1Ev");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");
        }
    }
}
