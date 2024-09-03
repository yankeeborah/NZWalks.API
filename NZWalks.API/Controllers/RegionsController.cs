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
