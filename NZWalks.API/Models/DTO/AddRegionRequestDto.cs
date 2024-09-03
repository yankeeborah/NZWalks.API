namespace NZWalks.API.Models.DTO
{
    public class AddRegionRequestDto
    {
        //no id because it is identity
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; } //nullable means can be null
    }
}
