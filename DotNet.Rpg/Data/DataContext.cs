using DotNet.Rpg.Data.Interface;
using DotNet.Rpg.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Rpg.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {

        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users{get;set;}
        public DbSet<Weapon> Weapons{get;set;}
        public DbSet<Skill> Skills{get;set;}
        public DbSet<CharacterSkill> CharacterSkills { get; set; }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            var affectedRecords = await base.SaveChangesAsync(cancellationToken);
            return affectedRecords;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
   
            modelBuilder.Entity<CharacterSkill>(entity =>
            {
                entity.HasKey(e=> new {e.CharacterId,e.SkillId});
            });
        }
    }
}
