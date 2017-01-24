namespace GUIExamASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class LunchClubContext : DbContext
    {
        // Your context has been configured to use a 'Week' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'GUIExamASP.Week' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Week' 
        // connection string in the application configuration file.
        public LunchClubContext() : base("name=LunchClub")
        {
            Database.SetInitializer<LunchClubContext>(new CreateDatabaseIfNotExists<LunchClubContext>());
        }

        public DbSet<Week> Weeks { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Participant> Participants { get; set; }


        
    }

    public class Week
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WeekId { get; set; }

        private List<Day> _weekDays;
        public virtual List<Day> WeekDays
        {
            get { return _weekDays ?? (_weekDays = new List<Day>()); }
            set { _weekDays = value; }
        }

    }

    public class Day
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DayId { get; set; }
        public string DayName { get; set; }

        private List<Participant> _participants;
        public List<Participant> Participants
        {
            get { return _participants ?? (_participants = new List<Participant>()); }
            set { _participants = value; }
        }
    }

    public class Participant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        private List<Day> _days;
        public List<Day> Days
        {
            get { return _days ?? (_days = new List<Day>()); }
            set { _days = value; }
        }
    }
    
}