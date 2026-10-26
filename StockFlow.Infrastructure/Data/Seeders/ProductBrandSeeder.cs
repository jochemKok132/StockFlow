using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class ProductBrandSeeder
    {
        public static void UseProductBrandSeeder(this DbContext context)
        {
            if (!context.Set<ProductBrand>().Any())
            {
                IEnumerable<ProductBrand> ProductBrands = new List<ProductBrand>()
                {
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000001"),
                        BrandName = "Flexa",
                        Description = "Dutch paint brand for walls, ceilings and wood, made by AkzoNobel.",
                        Website = "https://www.flexa.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@flexa.example",
                        CustomerServicePhone = "+31 20 555 0001"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000002"),
                        BrandName = "Sigma Coatings",
                        Description = "Professional paints and coatings for painters and do-it-yourselfers.",
                        Website = "https://www.sigma.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@sigmacoatings.example",
                        CustomerServicePhone = "+31 20 555 0002"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000003"),
                        BrandName = "Histor",
                        Description = "Paint for indoor and outdoor jobs, known for its colour range.",
                        Website = "https://www.histor.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@histor.example",
                        CustomerServicePhone = "+31 20 555 0003"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000004"),
                        BrandName = "Sikkens",
                        Description = "Premium paint and wood stain, mostly used by professional painters.",
                        Website = "https://www.sikkens.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@sikkens.example",
                        CustomerServicePhone = "+31 20 555 0004"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000005"),
                        BrandName = "Levis",
                        Description = "Belgian paint brand for walls, ceilings and woodwork.",
                        Website = "https://www.levis.info",
                        Country = "Belgium",
                        ContactEmail = "service@levis.example",
                        CustomerServicePhone = "+31 20 555 0005"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000006"),
                        BrandName = "GAMMA",
                        Description = "The store's own brand for everyday hardware.",
                        Website = "https://www.gamma.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@gamma.example",
                        CustomerServicePhone = "+31 20 555 0006"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000007"),
                        BrandName = "Bosch",
                        Description = "Tools, accessories and fixings for home and professional use.",
                        Website = "https://www.bosch-diy.com",
                        Country = "Germany",
                        ContactEmail = "service@bosch.example",
                        CustomerServicePhone = "+31 20 555 0007"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000008"),
                        BrandName = "Makita",
                        Description = "Power tools and fixings for professionals.",
                        Website = "https://www.makita.nl",
                        Country = "Japan",
                        ContactEmail = "service@makita.example",
                        CustomerServicePhone = "+31 20 555 0008"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000009"),
                        BrandName = "Fischer",
                        Description = "Fixing systems: plugs, screws and anchors.",
                        Website = "https://www.fischer.nl",
                        Country = "Germany",
                        ContactEmail = "service@fischer.example",
                        CustomerServicePhone = "+31 20 555 0009"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000a"),
                        BrandName = "Facom",
                        Description = "Hand tools and hardware for workshops.",
                        Website = "https://www.facom.com",
                        Country = "France",
                        ContactEmail = "service@facom.example",
                        CustomerServicePhone = "+31 20 555 0010"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000b"),
                        BrandName = "Legrand",
                        Description = "Switches, sockets and electrical installation material.",
                        Website = "https://www.legrand.nl",
                        Country = "France",
                        ContactEmail = "service@legrand.example",
                        CustomerServicePhone = "+31 20 555 0011"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000c"),
                        BrandName = "Philips",
                        Description = "Lighting and electrical accessories.",
                        Website = "https://www.philips.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@philips.example",
                        CustomerServicePhone = "+31 20 555 0012"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000d"),
                        BrandName = "Schneider Electric",
                        Description = "Electrical installation material and energy management.",
                        Website = "https://www.se.com/nl",
                        Country = "France",
                        ContactEmail = "service@schneiderelectric.example",
                        CustomerServicePhone = "+31 20 555 0013"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000e"),
                        BrandName = "Niko",
                        Description = "Switches, sockets and home automation.",
                        Website = "https://www.niko.eu",
                        Country = "Belgium",
                        ContactEmail = "service@niko.example",
                        CustomerServicePhone = "+31 20 555 0014"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000f"),
                        BrandName = "ABB",
                        Description = "Electrical equipment, from sockets to cables.",
                        Website = "https://new.abb.com",
                        Country = "Switzerland",
                        ContactEmail = "service@abb.example",
                        CustomerServicePhone = "+31 20 555 0015"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000010"),
                        BrandName = "Praxis",
                        Description = "Building materials and wood for home projects.",
                        Website = "https://www.praxis.nl",
                        Country = "Netherlands",
                        ContactEmail = "service@praxis.example",
                        CustomerServicePhone = "+31 20 555 0016"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000011"),
                        BrandName = "Kronoply",
                        Description = "Wood-based panels such as OSB and multiplex.",
                        Website = "https://www.kronoply.com",
                        Country = "Germany",
                        ContactEmail = "service@kronoply.example",
                        CustomerServicePhone = "+31 20 555 0017"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000012"),
                        BrandName = "Egger",
                        Description = "Wood-based materials for construction and furniture.",
                        Website = "https://www.egger.com",
                        Country = "Austria",
                        ContactEmail = "service@egger.example",
                        CustomerServicePhone = "+31 20 555 0018"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000013"),
                        BrandName = "Steico",
                        Description = "Wood fibre insulation and construction wood.",
                        Website = "https://www.steico.com",
                        Country = "Germany",
                        ContactEmail = "service@steico.example",
                        CustomerServicePhone = "+31 20 555 0019"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000014"),
                        BrandName = "Timberpro",
                        Description = "Planed and impregnated timber for garden and construction.",
                        Website = "https://timberpro.example",
                        Country = "Netherlands",
                        ContactEmail = "service@timberpro.example",
                        CustomerServicePhone = "+31 20 555 0020"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000015"),
                        BrandName = "Hansgrohe",
                        Description = "Showers, taps and bathroom fittings.",
                        Website = "https://www.hansgrohe.nl",
                        Country = "Germany",
                        ContactEmail = "service@hansgrohe.example",
                        CustomerServicePhone = "+31 20 555 0021"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000016"),
                        BrandName = "Villeroy & Boch",
                        Description = "Ceramics and bathroom furniture.",
                        Website = "https://www.villeroy-boch.nl",
                        Country = "Germany",
                        ContactEmail = "service@villeroyboch.example",
                        CustomerServicePhone = "+31 20 555 0022"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000017"),
                        BrandName = "Grohe",
                        Description = "Taps, showers and sanitary systems.",
                        Website = "https://www.grohe.nl",
                        Country = "Germany",
                        ContactEmail = "service@grohe.example",
                        CustomerServicePhone = "+31 20 555 0023"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000018"),
                        BrandName = "Duravit",
                        Description = "Bathroom design: toilets, washbasins and shower trays.",
                        Website = "https://www.duravit.nl",
                        Country = "Germany",
                        ContactEmail = "service@duravit.example",
                        CustomerServicePhone = "+31 20 555 0024"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000019"),
                        BrandName = "Geberit",
                        Description = "Sanitary technology such as cisterns and drains.",
                        Website = "https://www.geberit.nl",
                        Country = "Switzerland",
                        ContactEmail = "service@geberit.example",
                        CustomerServicePhone = "+31 20 555 0025"
                    },
                };
                context.Set<ProductBrand>().AddRange(ProductBrands);
                context.SaveChanges();
            }
        }
    }
}