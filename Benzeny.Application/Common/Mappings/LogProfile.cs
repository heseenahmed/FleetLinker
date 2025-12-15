

using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Log;

namespace BenzenyMain.Application.Common.Mappings
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<LogForCreateDto, Log>();

        }
    }
}
