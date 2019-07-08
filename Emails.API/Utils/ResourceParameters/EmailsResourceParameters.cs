using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.Utils.ResourceParameters
{
    public class EmailsResourceParameters
    {
        private const int _maxPageSize = 50;

        private int _pageSize = _maxPageSize;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
            }
        }

    }
}
