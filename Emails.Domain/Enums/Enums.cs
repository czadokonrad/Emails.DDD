using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Enums
{
    public class Enums
    {
        public enum EmailAddressTypeEnum : byte
        {
            SenderEmail = 1,
            SenderName = 2,
            ReceiverEmail = 3,
            ReceiverName = 4,
            CCEmail = 5,
            CCName = 6,
            BCCEmail = 7,
            BCCName = 8,
            ResentCCEmail = 9,
            ResentCCName = 10,
            ResentBCCEmail = 11,
            ResentBCCName = 12,
            ReplyToEmail = 13,
            ReplyToName = 14,
            FromEmail = 15,
            FromName = 16,
            InReplyToEmail = 17,
            InReplyToName = 18,
            ResentFromEmail = 19,
            ResentFromName = 20,
            ResentReplyToEmail = 21,
            ResentReplyToName = 22,
            ResentSenderEmail = 23,
            ResentSenderName = 24,
            ResentToEmail = 25,
            ResentToName = 26

        }
    }
}
