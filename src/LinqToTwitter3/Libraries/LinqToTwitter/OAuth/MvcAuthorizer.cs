﻿#if !NETFX_CORE
using System;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : WebAuthorizer
    {
        public ActionResult BeginAuthorization()
        {
            return new MvcOAuthActionResult(this);
        }

        public new ActionResult BeginAuthorization(Uri callback)
        {
            this.Callback = callback;
            return new MvcOAuthActionResult(this);
        }
    }
}

#endif