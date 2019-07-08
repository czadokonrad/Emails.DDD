using AutoMapper;
using Emails.API.DTOs;
using Emails.API.DTOs.CreationDTOs;
using Emails.API.DTOs.UpdateDTOs;
using Emails.API.EFCoreRepos.Interfaces;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.API.Controllers
{
    [ApiController]
    [Route("api/emails/{emailId}/attachments")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IEmailsRepository _emailsRepository;
        private readonly IMapper _mapper;

        public AttachmentsController(IEmailsRepository emailsRepository, IMapper mapper)
        {
            _emailsRepository = emailsRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAttachmentsForEmail(long emailId)
        {
            Result<Maybe<Email>> emailWithAttachmentsResult =
                await _emailsRepository.FindByKeyIncludeAsync(emailId, default(CancellationToken), email => email.Attachments);

            if (emailWithAttachmentsResult.IsSuccess)
            {
                if (emailWithAttachmentsResult.Value.HasValue)
                {
                    return Ok(_mapper.Map<IEnumerable<AttachmentDto>>(emailWithAttachmentsResult.Value.Value.Attachments));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{attachmentid}", Name = "GetAttachmentForEmail")]
        public async Task<IActionResult> GetAttachmentForAuthor(long emailId, long attachmentId)
        {
            Result<Maybe<Email>> emailWithAttachmentResult =
                await _emailsRepository.FindByKeyIncludeAsync(emailId, default(CancellationToken), email => email.Attachments);

            if (emailWithAttachmentResult.IsSuccess)
            {
                if (emailWithAttachmentResult.Value.HasValue)
                {
                    Attachment searchedAttachment = emailWithAttachmentResult
                        .Value.Value.Attachments.SingleOrDefault(a => a.Id == attachmentId);

                    if (searchedAttachment != null)
                    {
                        return Ok(_mapper.Map<AttachmentDto>(searchedAttachment));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttachmentForEmail(long emailId, [FromBody] AttachmentForCreationDto attachmentDto)
        {
            if (attachmentDto == null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                Result<Maybe<Email>> emailResult = await _emailsRepository
                    .FindByKeyIncludeAsync(emailId, default(CancellationToken));

                if (emailResult.IsSuccess)
                {
                    if (emailResult.Value.HasValue)
                    {
                        Attachment attachmentEntity = _mapper.Map<Attachment>(attachmentDto);

                        emailResult.Value.Value
                            .AddAttachment(attachmentEntity.FileName, attachmentEntity.DiskPath, attachmentEntity.FileExtension, attachmentEntity.SizeInKB);

                        Result<Email> updatedObject = await _emailsRepository.UpdateAsync(emailId, emailResult.Value.Value, default(CancellationToken));

                        if (updatedObject.IsSuccess)
                        {
                            return CreatedAtRoute("GetAttachmentForEmail",
                                new { attachmentid = updatedObject.Value.Attachments.Single().Id },
                                _mapper.Map<AttachmentDto>(updatedObject.Value.Attachments.Single()));
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }


        [HttpDelete("{attachmentid}")]
        public async Task<IActionResult> DeleteAttachmentForEmail(long emailId, long attachmentId)
        {
            Result<Maybe<Email>> emailResult = await _emailsRepository
                .FindByKeyIncludeAsync(emailId, default(CancellationToken), E => E.Attachments);

            if (emailResult.IsSuccess)
            {
                if (emailResult.Value.HasValue)
                {
                    if (!emailResult.Value.Value.Attachments.Any(A => A.Id == attachmentId))
                        return NotFound();

                    Result attachmentDeleteReult = await _emailsRepository.DeleteAttachmentAsync(emailId, attachmentId);

                    if (attachmentDeleteReult.IsSuccess)
                    {
                        return NoContent();
                    }
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, attachmentDeleteReult.Error);
                }
                else
                    return NotFound();
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, emailResult.Error);
        }


        [HttpPut("{attachmentid}")]
        public async Task<IActionResult> UpdateAttachmentForEmail(long emailId, long attachmentId,
            [FromBody] AttachmentForUpdateDto attachment)
        {
            if (attachment == null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                Result<Maybe<Email>> emailResult = await _emailsRepository
               .                                    FindByKeyIncludeAsync(emailId, default(CancellationToken), E => E.Attachments);

                if (emailResult.IsSuccess)
                {
                    if (emailResult.Value.HasValue)
                    {
                        if (!emailResult.Value.Value.Attachments.Any(A => A.Id == attachmentId))
                            return NotFound();

                        Result attachmentUpdateResult = await _emailsRepository.UpdateAttachmentAsync(emailId, attachmentId, attachment);

                        if (attachmentUpdateResult.IsSuccess)
                        {
                            return NoContent();
                        }
                        else
                            return StatusCode(StatusCodes.Status500InternalServerError, attachmentUpdateResult.Error);
                    }
                    else
                        return NotFound();
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, emailResult.Error);
            }
            else
                return UnprocessableEntity(ModelState);
        }

    }
}