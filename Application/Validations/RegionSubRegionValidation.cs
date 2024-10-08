using Domain.Enums;
using Domain.Enums.Domain.Enums;
using System.Collections.Generic;

namespace Application.Validations
{
    public static class RegionSubRegionValidation
    {
        private static readonly Dictionary<Region, List<SubRegion>> regionSubRegionMapping = new Dictionary<Region, List<SubRegion>>()
        {
            { Region.EURO, new List<SubRegion> { SubRegion.Europe } },
            { Region.LAAPA, new List<SubRegion> { SubRegion.LatinAmerica, SubRegion.AsiaPacific, SubRegion.Africa } },
            { Region.NOAM, new List<SubRegion> { SubRegion.America,SubRegion.Canada } }
        };

        public static bool IsValidSubRegionForRegion(Region region, SubRegion subRegion)
        {
            return regionSubRegionMapping.TryGetValue(region, out var validSubRegions) && validSubRegions.Contains(subRegion);
        }

        // New method to get all subregions for a given region
        public static List<SubRegion> GetSubRegionsForRegion(Region region)
        {
            return regionSubRegionMapping.ContainsKey(region) ? regionSubRegionMapping[region] : new List<SubRegion>();
        }
    }
}
