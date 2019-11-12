using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2B.Dto
{
    public class MethodStatusDto
    {
        public string Error { get; set; }
        public int ReturnValue { get; set; }
        public string ReturnMsg { get; set; }
    }
}