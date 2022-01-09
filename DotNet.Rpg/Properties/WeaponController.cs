using DotNet.Rpg.Dtos;
using DotNet.Rpg.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Rpg.Properties
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weapon)
        {
            _weaponService = weapon;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddWeapon(AddWeaponDto addWeaponDto)
        {
            return Ok(await _weaponService.AddWeapon(addWeaponDto));
        }
    }
}
