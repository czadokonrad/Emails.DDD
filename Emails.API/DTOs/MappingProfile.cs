using AutoMapper;
using Emails.API.DTOs.CreationDTOs;
using Emails.API.DTOs.UpdateDTOs;
using Emails.API.DTOs.Utils.Converters;
using Emails.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Emails.API.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Email, EmailDto>()
                .ForMember(member => member.MessageHtml,
                memberOptions => memberOptions.ConvertUsing(new HtmlToEncodedStringConverter()));
            CreateMap<EmailForCreationDto, Email>();
            CreateMap<EmailForUpdateDto, Email>();
            CreateMap<Email, EmailForUpdateDto>();

            CreateMap<Attachment, AttachmentDto>();
            CreateMap<AttachmentForCreationDto, Attachment>();
            CreateMap<AttachmentForUpdateDto, Attachment>();

        }
    }
}
