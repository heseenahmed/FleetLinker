using AutoMapper;

namespace Benzeny.Application.Common.Mappings
{
    public interface ImapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}