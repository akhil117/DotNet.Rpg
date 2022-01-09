
using DotNet.Rpg.Enum;
using System.Collections.Generic;

namespace DotNet.Rpg.Dtos
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public Roles Class { get; set; }
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights {get;set;}
        public int Victories {get;set;}
        public int Defeats {get;set;}
    }
}
