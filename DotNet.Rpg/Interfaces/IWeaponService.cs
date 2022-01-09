using System.Threading.Tasks;
using DotNet.Rpg.Dtos;
using DotNet.Rpg.Models;

namespace DotNet.Rpg.Interfaces
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}