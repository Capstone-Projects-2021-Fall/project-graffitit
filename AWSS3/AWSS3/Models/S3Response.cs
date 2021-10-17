using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Util;

namespace AWSS3
{
    public class S3Response
    {
        public HttpStatusCode Status { get; set;}

        public string Message {get; set;}
    }
    
}


