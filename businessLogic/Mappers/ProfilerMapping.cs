﻿using AutoMapper;
using businessLogic.ModelViews;
using DataLayer.Entities;

namespace businessLogic.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for ProfileViewModel
            CreateMap<ApplicationUser, ProfileViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.OldThan18, opt => opt.MapFrom(src => src.Age.HasValue && src.Age > 18))
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Role assignment done separately

            // Mapping for CreateUserViewModel to ApplicationUser
            CreateMap<CreateUserViewModel, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password handled by Identity framework

            // Mapping for RegisterViewModel to ApplicationUser
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password handled by Identity framework
        }
    }
}
