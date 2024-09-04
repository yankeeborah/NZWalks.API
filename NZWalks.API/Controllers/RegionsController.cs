using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")] //route url https://localhost:1234/api/Regions
    [ApiController] //this controller is for api use
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext,IRegionRepository regionRepository,IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           //Get data from database : domain model
            var regionsDomain = await regionRepository.GetAllAsync();

            //map domain model to DTO
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain) {
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = regionDomain.Id,
            //        Code = regionDomain.Code,
            //        Name = regionDomain.Name,
            //        RegionImageUrl=regionDomain.RegionImageUrl

            //    });
            //}
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain); // doing the task of mapping
            return Ok(regionsDto);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //Get region Domain model from Database
            //var region = dbContext.Regions.Find(id); // we can also use firstordefault 
            //find is use with unique key only
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null) {                    
                return NotFound();
            }
            //map region domain model to region DTO
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomain); // doing the task of mapping

            //return DTO back to client
            return Ok(regionDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddRegionRequestDto addRegionRequestDto)
        {
            //Map DTO to Domain model
            //var regionDomainModel = new Region
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto); // doing the task of mapping


            //Use Domain model to create Region
            //await dbContext.Regions.AddAsync(regionDomainModel);
            //await dbContext.SaveChangesAsync();
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            //map domain back to DTO
            //var regionDto = new RegionDto { 
            //Id= regionDomainModel.Id,
            //Code = regionDomainModel.Code,
            //Name = regionDomainModel.Name,
            //RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel); // doing the task of mapping

            //want to get back the record in client
            return CreatedAtAction(nameof(GetById),new { id = regionDomainModel.Id},regionDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateRegionRequestDto updateRegionRequestDto)
        {
            //map dto to domain model
            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            if(regionDomainModel == null)
            {
                return NotFound();
            }
            //update the Domain Model


            //Map Domain model to  DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel); // doing the task of mapping

            return Ok(regionDto);

 

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }


            //return deleted region
            //mapping before returning
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            //return DTO back to client
            return Ok(regionDto);
        }
    }

    //DTO - Data transfer object
    //Used to transfer data between different layers of the application
    //typically contain a subset of the properties in the domain model
    //Eg transferring data over a network
    // CLIENT <--> DTO <--> API <--> Domain Model <--> Database
    //Benifits of using DTO -
    //Seperation of Concern : only useful properties from Domain
    //Performance : retrieve data only needed 
    //Security : limiting the amount of data exposed to client(restricting sensitive data) 
    //Versioning : update the API without braeking the existing client
}
