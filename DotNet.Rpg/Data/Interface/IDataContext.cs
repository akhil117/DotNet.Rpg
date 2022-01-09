using DotNet.Rpg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Rpg.Data.Interface
{
    public interface IDataContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users{get;set;}
        public DbSet<Weapon> Weapons{get;set;}
        public DbSet<Skill> Skills{get;set;}

        public DbSet<CharacterSkill> CharacterSkills    {get;set;}

        Task<int> SaveAsync(CancellationToken cancellationToken);
        EntityEntry Entry(object entity);

    }
}
