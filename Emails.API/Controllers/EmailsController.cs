using AutoMapper;
using Emails.API.DTOs;
using Emails.API.DTOs.CreationDTOs;
using Emails.API.DTOs.UpdateDTOs;
using Emails.API.EFCoreRepos.Interfaces;
using Emails.API.Utils;
using Emails.API.Utils.ResourceParameters;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Emails.API.Utils.Enums.Enums;

namespace Emails.API.Controllers
{
    [Route("api/emails")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailsRepository _emailsRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        private string CreateEmailsResourceUri(EmailsResourceParameters emailsResourceParameters, ResourceUriType resourceType)
        {


            switch (resourceType)
            {
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByAction("GetEmailsPage", "Emails",
                        new
                        {
                            pageNumber = emailsResourceParameters.PageNumber + 1,
                            pageSize = emailsResourceParameters.PageSize
                        });
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByAction("GetEmailsPage", "Emails",
                      new
                      {
                          pageNumber = emailsResourceParameters.PageNumber - 1,
                          pageSize = emailsResourceParameters.PageSize
                      });
                default:
                    return _linkGenerator.GetPathByAction("GetEmailsPage", "Emails",
                     new
                     {
                         pageNumber = emailsResourceParameters.PageNumber,
                         pageSize = emailsResourceParameters.PageSize
                     });
            }
        }


        public EmailsController(IEmailsRepository emailsRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _emailsRepository = emailsRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }


        [HttpGet(Name = "GetEmailsPage")]
        public async Task<IActionResult> GetEmailsPage([FromQuery] EmailsResourceParameters emailsResourceParameters)
        {
            Result<Maybe<PagedList<Email>>> emailsPageResult = await _emailsRepository.GetPageAsync(emailsResourceParameters);

            if (emailsPageResult.IsSuccess)
            {
                if (emailsPageResult.Value.HasValue)
                {

                    string previousPageLink = emailsPageResult.Value.Value.HasPreviousPage ?
                        CreateEmailsResourceUri(emailsResourceParameters, ResourceUriType.PreviousPage) : null;

                    string nextPageLink = emailsPageResult.Value.Value.HasNextPage ?
                        CreateEmailsResourceUri(emailsResourceParameters, ResourceUriType.NextPage) : null;

                    return Ok(emailsPageResult.Value.Value.Select(E => _mapper.Map<EmailDto>(E)));

                }
                else
                    return NotFound();
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError);

        }


        [HttpGet("{id}", Name = "GetEmail")]
        public async Task<IActionResult> GetEmail([FromRoute] long id)
        {
            Result<Maybe<Email>> emailResult = await _emailsRepository.FindByKeyAsync(id, default(CancellationToken));

            if (emailResult.IsSuccess)
            {
                if (emailResult.Value.HasValue)
                {
                    return Ok(_mapper.Map<EmailDto>(emailResult.Value.Value));
                }
                else
                    return NotFound();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostEmail([FromBody] EmailForCreationDto email)
        {

            if (email == null)
                return BadRequest();

            if (ModelState.IsValid)
            {

                Email emailEntity = _mapper.Map<Email>(email);

                Result<Email> emailToReturn = await _emailsRepository.InsertAsync(emailEntity, default(CancellationToken));

                if (emailToReturn.IsSuccess)
                {
                    return CreatedAtRoute("GetEmail", new { id = emailToReturn.Value.Id }, _mapper.Map<EmailDto>(emailToReturn.Value));
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmail([FromRoute] long id)
        {
            Result<Maybe<Email>> emailToDeleteResult =
                await _emailsRepository.FindByKeyAsync(id, default(CancellationToken));

            if (emailToDeleteResult.IsSuccess)
            {
                if (emailToDeleteResult.Value.HasValue)
                {
                    Result deleteResult = await _emailsRepository.DeleteAsync(id, default(CancellationToken));

                    if (deleteResult.IsSuccess)
                    {
                        return NoContent();
                    }
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, deleteResult.Error);
                }
                else
                    return NotFound();
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, emailToDeleteResult.Error);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmail([FromRoute] long id,
            [FromBody] JsonPatchDocument<EmailForUpdateDto> patchDoc)

        {

            if (patchDoc == null)
                return BadRequest();

            Result<Maybe<Email>> emailToPatchResult =
               await _emailsRepository.FindByKeyAsync(id, default(CancellationToken));

            if (emailToPatchResult.IsSuccess)
            {
                if (emailToPatchResult.Value.HasValue)
                {

                    EmailForUpdateDto emailToPatch = _mapper.Map<EmailForUpdateDto>(emailToPatchResult.Value.Value);

                    patchDoc.ApplyTo(emailToPatch, ModelState);

                    TryValidateModel(emailToPatch);

                    if (!ModelState.IsValid)
                        return UnprocessableEntity(ModelState);

                    Email emailEntity = _mapper.Map<Email>(emailToPatch);
                    emailEntity.GetType().BaseType.BaseType.GetProperty(nameof(Email.Id)).SetValue(emailEntity, emailToPatchResult.Value.Value.Id);
                    Result updateResult = await _emailsRepository.UpdateAsync(id, emailEntity, default(CancellationToken));

                    if (updateResult.IsSuccess)
                    {
                        return NoContent();
                    }
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
                }
                else
                    return NotFound();
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, emailToPatchResult.Error);
        }
    }
}
