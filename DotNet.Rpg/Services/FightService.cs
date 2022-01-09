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
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Rpg.Services
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly IDataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICharacterService _characterService;
        public FightService(IMapper mapper, IDataContext dataContext, IHttpContextAccessor httpContextAccessor,ICharacterService characterService)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _characterService = characterService;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightResultDto)
        {
            var response = new ServiceResponse<FightResultDto>() {
                Data = new FightResultDto()
            };
            try
            {
                List<Character> characters = await _dataContext.Characters
                                            .Include(x => x.Weapon)
                                            .Include(x => x.CharacterSkills)
                                            .ThenInclude(x => x.Skill)
                                            .Where(x => fightResultDto.CharacterIds.Contains(x.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach(var attacker in characters)
                    {
                        List<Character> opponents = await _dataContext.Characters.Where(x => !fightResultDto.CharacterIds.Contains(x.Id)).ToListAsync();

                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            UseWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                           damage= UseSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }
                        response.Data.Log.Add($"Attacker: {attacker.Name} attacked Opponent: {opponent.Name} using attack/skill: {attackUsed} with {(damage>=0?damage:0)} Damage");
                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"Attacker: {attacker.Name} defeated attacked Opponent: {opponent.Name} ");
                            break;
                        }
                    }
                }

                characters.ForEach(x =>
                {
                    x.Fights++;
                    x.HitPoints = 100;
                });

                _dataContext.Characters.UpdateRange(characters);
                await _dataContext.SaveAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                response.message = e.Message;
                response.success = false;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _dataContext.Characters.Include(x => x.CharacterSkills).ThenInclude(x => x.Skill).FirstOrDefaultAsync(x => x.Id == request.AttackerId);
                Character opponent = await _dataContext.Characters.FirstOrDefaultAsync(x => x.Id == request.OpponentId);

                var characterSkill = (attacker.CharacterSkills.FirstOrDefault(x => x.SkillId == request.AttackerSkillId));

                if (characterSkill == null)
                {
                    response.message = "Invalid Skill";
                    response.success = false;
                    return response;
                }
                int damage = UseSkillAttack(attacker, opponent, characterSkill);
                if (opponent.HitPoints <= 0)
                    response.message = $"{opponent.Name} has been defeated";

                _dataContext.Entry(opponent).State = EntityState.Modified;
                await _dataContext.SaveAsync(CancellationToken.None);

                response.Data = new AttackResultDto()
                {
                    Damage = damage,
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    AttackerHP = attacker.HitPoints
                };
            }
            catch (Exception e)
            {
                response.message = e.Message;
                response.success = false;
            }

            return response;
        }

        private static int UseSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= damage - new Random().Next(opponent.Defence);
            if (damage > 0)
            {
                opponent.HitPoints = opponent.HitPoints - damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _dataContext.Characters.Include(x => x.Weapon).FirstOrDefaultAsync(x => x.Id == request.AttackerId);
                Character opponent = await _dataContext.Characters.Include(x => x.Weapon).FirstOrDefaultAsync(x => x.Id == request.OpponentId);

                int damage = UseWeaponAttack(attacker, opponent);
                if (opponent.HitPoints <= 0)
                    response.message = $"{opponent.Name} has been defeated";

                _dataContext.Entry(opponent).State = EntityState.Modified;
                await _dataContext.SaveAsync(CancellationToken.None);

                response.Data = new AttackResultDto()
                {
                    Damage = damage,
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    AttackerHP = attacker.HitPoints
                };

            }
            catch (Exception e)
            {
                response.message = e.Message;
                response.success = false;
            }
            return response;
        }

        private static int UseWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage = damage - new Random().Next(opponent.Defence);

            if (damage > 0)
            {
                opponent.HitPoints = opponent.HitPoints - damage;
            }

            return damage;
        }
    }
}
