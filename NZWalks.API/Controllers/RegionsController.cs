using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")] //route url https://localhost:1234/api/Regions
    [ApiController] //this controller is for api use
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
           //Get data from database : domain model
            var regionsDomain = dbContext.Regions.ToList();

            //map domain model to DTO
            var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain) {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl=regionDomain.RegionImageUrl

                });
            }
            return Ok(regionsDto);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            //Get region Domain model from Database
            //var region = dbContext.Regions.Find(id); // we can also use firstordefault 
            //find is use with unique key only
            var regionDomain = dbContext.Regions.FirstOrDefault(x => x.Id == id);

            if (regionDomain == null) {                    
                return NotFound();
            }
            //map region domain model to region DTO
            var regionDto = new RegionDto()
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl
            };
            //return DTO back to client
            return Ok(regionDto);
        }

        [HttpPost]
        public IActionResult Create(AddRegionRequestDto addRegionRequestDto)
        {
            //Map DTO to Domain model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            //Use Domain model to create Region
            dbContext.Regions.Add(regionDomainModel);
            dbContext.SaveChanges();

            //map domain back to DTO
            var regionDto = new RegionDto { 
            Id= regionDomainModel.Id,
            Code = regionDomainModel.Code,
            Name = regionDomainModel.Name,
            RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            //want to get back the record in client
            return CreatedAtAction(nameof(GetById),new { id = regionDomainModel.Id},regionDto);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }
            //update the Domain Model
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;
            dbContext.SaveChanges();
            //Map Domain model to  DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok(regionDto);

 

        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var regionDomainModel = dbContext.Regions.FirstOrDefault(x=>x.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            dbContext.Regions.Remove(regionDomainModel);
            dbContext.SaveChanges();

            //return deleted region
            var regionDto = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
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
