using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Emails.API.DTOs.Utils.Converters
{
    public class HtmlToEncodedStringConverter : IValueConverter<string, string>
    {

        public string Convert(string sourceMember, ResolutionContext context)
        {
            return HttpUtility.HtmlEncode(sourceMember);
        }
    }
}
