using Microsoft.EntityFrameworkCore;
using QuestTracker.Models;
using System;
using System.IO;

namespace QuestTracker.Data
{
    public class QuestTrackerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Quest> Quests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Databasen ligger i project-mappen
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "questtracker.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public void InitializeDatabase()
        {
            Database.EnsureCreated();
        }
    }
}