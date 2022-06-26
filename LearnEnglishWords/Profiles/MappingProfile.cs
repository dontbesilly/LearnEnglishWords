using AutoMapper;
using LearnEnglishWords.Dto;
using LearnEnglishWords.Models;

namespace LearnEnglishWords.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WordDto, Word>().ReverseMap();
    }
}