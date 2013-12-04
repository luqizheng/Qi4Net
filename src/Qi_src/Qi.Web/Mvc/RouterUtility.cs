﻿using System;
using System.Web;
using System.Web.Routing;

namespace Qi.Web.Mvc
{
    public class RouterUtility
    {
        public static RouteData GetRouteDataByUrl(string url)
        {
            try
            {
                if (!url.StartsWith("~"))
                {
                    url = "~" + url;
                }
                RouteData data = RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase mockHttpRequestBase;

            public RewritedHttpContextBase(string appRelativeUrl)
            {
                mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }


            public override HttpRequestBase Request
            {
                get { return mockHttpRequestBase; }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string appRelativeUrl;

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this.appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return appRelativeUrl; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }
            }
        }
    }
}