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
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly IDataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(IMapper mapper, IDataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int getUserId() => int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier).Value);

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto addCharacter)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(addCharacter);

            character.User = await _dataContext.Users.FirstOrDefaultAsync(x=>x.Id==getUserId());

            await _dataContext.Characters.AddAsync(character);
            await _dataContext.SaveAsync(CancellationToken.None);
            response.Data = await _dataContext.Characters.Where(x=>x.User.Id==getUserId()).Select(x => _mapper.Map<GetCharacterDto>(x)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            int userId = getUserId();
            response.Data = await _dataContext.Characters.Include(c => c.Weapon).Include(c => c.CharacterSkills).ThenInclude(c => c.Skill).Where(x=> x.User.Id==userId).Select(x => _mapper.Map<GetCharacterDto>(x)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetById(int id)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            response.Data = _mapper.Map<GetCharacterDto>(await _dataContext.Characters.Include(c => c.Weapon).Include(c => c.CharacterSkills).ThenInclude(c => c.Skill).FirstOrDefaultAsync(x => x.Id == id));
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dataContext.Characters.Where(x=>x.User.Id==getUserId()).SingleOrDefaultAsync(x => x.Id == updateCharacterDto.Id);

                if (character != null)
                {
                    character.Intelligence = updateCharacterDto.Intelligence;
                    character.Strength = updateCharacterDto.Strength;
                    character.Class = updateCharacterDto.Class;
                    character.Defence = updateCharacterDto.Defence;
                    character.HitPoints = updateCharacterDto.HitPoints;
                    character.Name = updateCharacterDto.Name;

                    response.Data = _mapper.Map<GetCharacterDto>(character);

                    _dataContext.Entry(character).State = EntityState.Modified;
                    await _dataContext.SaveAsync(CancellationToken.None);

                    response.Data = _mapper.Map<GetCharacterDto>(character);

                    return response;
                }
                else
                {
                    response.message = "Character not found";
                    response.success = false;

                    return response;
                }

            }
            catch (Exception e)
            {
                response.success = false;
                response.message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var character = await _dataContext.Characters.Where(x=>x.User.Id==getUserId()).SingleOrDefaultAsync(x => x.Id == id);
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

            if (character != null)
            {

                _dataContext.Entry(character).State = EntityState.Deleted;
                await _dataContext.SaveAsync(CancellationToken.None);

                response.Data = await _dataContext.Characters.Where(x => x.User.Id == getUserId()).Select(x => _mapper.Map<GetCharacterDto>(x)).ToListAsync();
            }
            else
            {
                response.message = "Character not found";
                response.success = false;
            }

            return response;
        }

    }
}
