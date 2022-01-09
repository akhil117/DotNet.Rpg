using AutoMapper;
using DotNet.Rpg.Data.Interface;
using DotNet.Rpg.Dtos;
using DotNet.Rpg.Interfaces;
using DotNet.Rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Rpg.Services
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly IDataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterSkillService(IMapper mapper, IDataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private int getUserId() => int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dataContext.Characters.FirstOrDefaultAsync(x => x.User.Id == getUserId() && x.Id == addCharacterSkillDto.CharacterId);
                var skill = await _dataContext.Skills.FirstOrDefaultAsync(x => x.Id == addCharacterSkillDto.SkillId);
                if (character != null && skill != null)
                {
                    var addCharacterSkill = new CharacterSkill
                    {
                        CharacterId = addCharacterSkillDto.CharacterId,
                        SkillId = addCharacterSkillDto.SkillId
                    };

                    await _dataContext.CharacterSkills.AddAsync(addCharacterSkill);
                    await _dataContext.SaveAsync(CancellationToken.None);

                    var result = await _dataContext.Characters.Include(c=>c.Weapon).Include(c=>c.CharacterSkills).ThenInclude(c=>c.Skill).FirstOrDefaultAsync(x => x.User.Id == getUserId() && x.Id == addCharacterSkillDto.CharacterId);

                    response.Data = _mapper.Map<GetCharacterDto>(result);
                    
                }
                else
                {
                    response.success = false;
                    response.message = "Invalid Details";
                }
            }catch(Exception e)
            {
                response.success = false;
                response.message = e.Message;
            }
            return response;
        }
    }
}
