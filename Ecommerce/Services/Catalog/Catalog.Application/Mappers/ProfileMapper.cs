using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Application.Mappers;

public class ProfileMapper : Profile
{
    public ProfileMapper()
    {
        CreateMap<ProductBrand, BrandResponse>().ReverseMap();
        CreateMap<Product, ProductResponse>().ReverseMap();
        CreateMap<TypeResponse, ProductType>().ReverseMap();
        CreateMap<Product, CreateProductCommand>().ReverseMap();
        CreateMap<UpdateProductCommand, Product>().ReverseMap();
        CreateMap<Pagination<Product>, Pagination<ProductResponse>>();
    }
}