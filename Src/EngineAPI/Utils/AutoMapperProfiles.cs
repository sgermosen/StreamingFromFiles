using AutoMapper;
using EngineAPI.DTOs;
using EngineAPI.Entities;
using NetTopologySuite.Geometries;

namespace EngineAPI.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            //CreateMap<Immunization, ImmunizationDTO>()
            //    .ForMember(x => x.LaboratoryName, x => x.MapFrom(d => d.Laboratory.Name))
            //    .ForMember(x => x.VaccineName, x => x.MapFrom(d => d.Vaccine.Name));

            //CreateMap<ImmunizationDTO, Immunization>();

            //CreateMap<DniOrCardDTO, Image>().ReverseMap();
            ////  CreateMap<Immunization, ImmunizationDTO>().ReverseMap();

            //CreateMap<ImmunizationCreationDTO, Immunization>();
            ////.ForMember(x => x.CardPicture,
            ////                options => options.Ignore());//  we ignore one or various properties than we want to treat as a diferent way  

            CreateMap<ApplicationUser, UserDTO>();

            //Map Details from file of another part of class
            //CreateMap<DomainObjectCreationDTO, DomainObject>()
            // .ForMember(x => x.Photo,
            //                 options => options.Ignore())
            // .ForMember(x => x.DomainObjectDetails1DTO, options => options.MapFrom(MapDomianObjectDetails1))
            // .ForMember(x => x.DomainObjectDetails2DTO, options => options.MapFrom(MapDomianObjectDetails2))

            //the reserse part of the one before 
            //CreateMap<DomainObject, DomainObjectDTO>()
            //            .ForMember(x => x.Details1, options => options.MapFrom(MapDomainObjectDetails1))
            //            .ForMember(x => x.Details2, options => options.MapFrom(MapDomainObjectDetails2)) 

        }




    }
}
