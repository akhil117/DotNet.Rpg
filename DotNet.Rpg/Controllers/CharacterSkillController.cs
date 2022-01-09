using DotNet.Rpg.Dtos;
using DotNet.Rpg.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Rpg.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            _characterSkillService = characterSkillService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            return Ok(await _characterSkillService.AddCharacterSkill(addCharacterSkillDto));
        }
    }
}
