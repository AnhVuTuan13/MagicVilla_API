using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaNumber")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _repository;
        private readonly IVillaRepository _villaRepository;
        public readonly IMapper _mapper;

        public VillaNumberAPIController(IVillaNumberRepository repository, IMapper mapper, IVillaRepository villaRepository)
        {
            _villaRepository = villaRepository;
            _repository = repository;
            _mapper = mapper;
            _response = new();
        }

       /* [MapToApiVersion("2.0")]*/
        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[] { "DotNet", "No21" };
        }


    }
}
