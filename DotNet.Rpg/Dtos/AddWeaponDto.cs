using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Rpg.Dtos
{
    public class AddWeaponDto
    {
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
    }
}
