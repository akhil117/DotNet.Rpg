using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DotNet.Rpg.Data.Interface;
using DotNet.Rpg.Dtos;
using DotNet.Rpg.Interfaces;
using DotNet.Rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Rpg.Services
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly IDataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WeaponService(IMapper mapper, IDataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int getUserId() => int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            var weapon = _mapper.Map<Weapon>(newWeapon);
            var response = new ServiceResponse<GetCharacterDto>();


            try
            {
                if (await ValidateCharacterId(getUserId(), newWeapon.CharacterId))
                {
                    await _dataContext.Weapons.AddAsync(weapon);
                    await _dataContext.SaveAsync(CancellationToken.None);

                    var character = await _dataContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                    c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
                    var wep = _mapper.Map<GetWeaponDto>(character.Weapon);
                    var output = _mapper.Map<GetCharacterDto>(character);
                   // output.GetWeaponDto = wep;
                    response.success = true;
                    response.message = "Valid Character Id";
                    response.Data = output;
                }
                else
                {
                    response.message = "Invalid Character Id";
                    response.success = false;
                }

                return response;
            }catch(Exception e)
            {
                response.success = false;
                response.message = e.Message;
                return response;
            }
        }


        private async Task<bool> ValidateCharacterId(int userId,int characterId)
        {
            var character = await _dataContext.Characters.Include(x=>x.User).SingleOrDefaultAsync(x => x.Id == characterId);
            if(character?.User.Id == userId)
            {
                return true;
            }
            return false;
        }
    }
}