using AutoMapper;
using DotNet.Rpg.Dtos;
using DotNet.Rpg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Rpg.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>()
                .ForMember(x=>x.Weapon, opt=>opt.MapFrom(src=> src.Weapon))
                .ForMember(x=>x.Skills, opt=>opt.MapFrom(src=>src.CharacterSkills.Select(cs=>cs.Skill)));
            CreateMap<AddCharacterDto, Character>();
            CreateMap<AddWeaponDto,Weapon>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }
    }
}
