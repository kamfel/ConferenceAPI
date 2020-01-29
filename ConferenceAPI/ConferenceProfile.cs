using AutoMapper;
using ConferenceAPI.Core.Models;
using ConferenceAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI
{
    public class ConferenceProfile : Profile
    {
        public ConferenceProfile()
        {
            UserMappings();
            RoomMappings();

            CreateMap<Reservation, ReservationDTO>()
                .ForMember(dto => dto.StartingTime, cfg => cfg.MapFrom(src => src.Block.StartTime))
                .ForMember(dto => dto.EndingTime, cfg => cfg.MapFrom(src => src.Block.EndTime))
                .ForMember(dto => dto.UserEmail, cfg => cfg.MapFrom(src => src.User.Email));
        }

        private void UserMappings()
        {
            CreateMap<UserRegisterDTO, User>()
                .ForMember(d => d.Reservations, cfg => cfg.Ignore())
                .ForMember(d => d.UserPermissions, cfg => cfg.Ignore())
                .ForMember(d => d.Id, cfg => cfg.Ignore())
                .ForMember(d => d.Password, cfg => cfg.Ignore());

            CreateMap<User, UserDTO>();

            CreateMap<User, UserInfoDTO>();
        }

        private void RoomMappings()
        {
            CreateMap<Room, RoomDTO>();

            CreateMap<Room, RoomDetailsDTO>()
                .ForMember(dto => dto.Layout, cfg => cfg.Ignore())
                .ForMember(dto => dto.Devices, cfg => cfg.Ignore())
                .ReverseMap();
        }
    }
}
