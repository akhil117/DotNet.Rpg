using DotNet.Rpg.Dtos;
using DotNet.Rpg.Interfaces;
using DotNet.Rpg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNet.Rpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CharacterController : ControllerBase
    {
       
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
           _characterService = characterService;
        }


        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddCharacterDto addCharacterDt)
        {
            return Ok(await _characterService.AddCharacter(addCharacterDt));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            return Ok(await _characterService.GetById(id));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCharacterById(UpdateCharacterDto updateCharacterDto)
        {
            return Ok(await _characterService.UpdateCharacter(updateCharacterDto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCharacterById(int id)
        {
            return Ok(await _characterService.DeleteCharacter(id));
        }
    }
}
