using Common.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Application1.Structs;

namespace Application1
{
    public static class UrlHelper
    {
        public static string BuildUrl(string @base, string name)
        {
            var encodedName = HttpUtility.UrlEncode(name);
            var completeUrl = $"{@base}?{QueryKey.NAME}={encodedName}";

            return completeUrl;
        }
    }
}
