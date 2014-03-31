using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HuobiAPI.Lib
{
    public class WebClientPlus : WebClient
    {
        private int timeout = 0;
        private WebResponse webResponse = null;

        public WebClientPlus(int _timeout)
            : base()
        {
            this.timeout = _timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest _request = (HttpWebRequest)base.GetWebRequest(address);
            _request.Timeout = this.timeout;
            _request.ReadWriteTimeout = this.timeout;
            return _request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            this.webResponse = base.GetWebResponse(request);
            return this.webResponse;
        }

        public HttpStatusCode HttpStatusCode
        {
            get
            {
                if (this.webResponse == null)
                {
                    return HttpStatusCode.NotImplemented;
                }
                else
                {
                    return ((HttpWebResponse)this.webResponse).StatusCode;
                }
            }
        }
    }
}
