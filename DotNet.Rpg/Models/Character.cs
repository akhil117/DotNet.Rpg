using System.Collections.Generic;
using DotNet.Rpg.Enum;

namespace DotNet.Rpg.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public Roles Class { get; set; } = Roles.Knight;
        public User User { get; set; }
        public Weapon Weapon {get;set;}
        public List<CharacterSkill> CharacterSkills {get;set;}
        public int Fights {get;set;}
        public int Victories {get;set;}
        public int Defeats {get;set;}
    }
}
