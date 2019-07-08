using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Emails.API.DTOs;
using Emails.API.DTOs.CreationDTOs;
using Emails.API.EFCoreRepos.Interfaces;
using Emails.API.Utils.CustomModelBinders;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace Emails.API.Controllers
{
    [Route("api/emailscollection")]
    [ApiController]
    public class EmailsCollectionController : ControllerBase
    {
        private readonly IEmailsRepository _emailsRepository;
        private readonly IMapper _mapper;

        public EmailsCollectionController(IEmailsRepository emailsRepository, IMapper mapper)
        {
            _emailsRepository = emailsRepository;
            _mapper = mapper;
        }


        [HttpGet("({ids})", Name = "GetEmailsCol")]
        public async Task<IActionResult> GetEmailsCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<long> ids)
        {
            if (ids == null)
                return BadRequest();

            Result<Maybe<IEnumerable<Email>>> emailsCollectionResult = await _emailsRepository.AllWhereAsync(E => ids.Contains(E.Id), default(CancellationToken));

            if (emailsCollectionResult.IsSuccess)
            {
                if (emailsCollectionResult.Value.HasValue)
                {
                    if(ids.Count() != emailsCollectionResult.Value.Value.Count())
                    {
                        return NotFound();
                    }

                    return Ok(emailsCollectionResult.Value.Value.Select(E => _mapper.Map<EmailDto>(E)));
                }
                else
                    return NotFound();
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, emailsCollectionResult.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailsCollection([FromBody] IEnumerable<EmailForCreationDto> emailsCollection)
        {
            if (emailsCollection == null)
                return BadRequest();

            if(ModelState.IsValid)
            {
                IEnumerable<Email> emailEntities = emailsCollection.Select(e => _mapper.Map<Email>(e));

                Result<IEnumerable<Email>> emailCollectionInsertResult = 
                    await _emailsRepository.InsertRangeAsync(emailEntities, default(CancellationToken));

                if(emailCollectionInsertResult.IsSuccess)
                {
                    return CreatedAtRoute("GetEmailsCol",
                        new { ids = string.Join(",", emailCollectionInsertResult.Value.Select(E => E.Id)) },
                        emailCollectionInsertResult.Value.Select(E => _mapper.Map<EmailDto>(E)));
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, emailCollectionInsertResult.Error);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }




    }
}
