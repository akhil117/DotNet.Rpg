using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Rpg.Dtos
{
    public class SkillAttackDto
    {
        public int AttackerId { get; set; }
        public int AttackerSkillId { get; set; }
        public int OpponentId { get; set; }

    }
}
