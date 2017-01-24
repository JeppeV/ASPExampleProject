using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GUIExamASP.Models;

namespace GUIExamASP.Controllers
{
    public class HomeController : Controller
    {
        private string[] WEEK_DAYS = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };

        [HttpGet]
        public ActionResult Index()
        {
            
            List<Participant> participants;
            using (var context = new LunchClubContext())
            {
                participants = context.Participants.ToList();
            }
            ViewBag.Title = "FrokostKlubben";
            return View("Index", participants);

        }

        [HttpPost]
        public ActionResult Index(string name)
        {
            ActionResult result;
            List<Participant> participants;
            name = name.Trim();
            using (var context = new LunchClubContext())
            {
                if (attemptAddNewParticipant(name, context))
                {
                    participants = context.Participants.ToList();
                    result = View("Index", participants);
                }else
                {
                    result = new HttpStatusCodeResult(202, "Medlemmet findes allerede");
                }
                
            }
            ViewBag.Title = "FrokostKlubben";
            return result;
        }

        private bool attemptAddNewParticipant(string name, LunchClubContext context)
        {
            Participant participant = context.Participants.Where(p => p.Name == name).FirstOrDefault();
            if(participant == null)
            {
                // participant does not exist - create new participant
                addNewParticipant(name, context);
                return true;
            }
            return false;
        }
        
        private void addNewParticipant(string name, LunchClubContext context)
        {
            Participant participant = new Participant { Id = Guid.NewGuid(), Name = name };
            context.Participants.Add(participant);
            foreach(Day day in context.Days)
            {
                day.Participants.Add(participant);
            }
            context.SaveChanges();
        }  

        public ActionResult WeekSelection()
        {
            ViewBag.Title = "FrokostKlubben";
            return View("WeekSelection");
        }

        


        public ActionResult Week()
        {
            if (Request.Form["weeknumber"] == null) return View("Index");
            var weeknumber = int.Parse(Request.Form["weeknumber"]);
            Week week;
            using (var context = new LunchClubContext())
            {
                //eager loading of WeekDays collection property
                week = context.Weeks.Include("WeekDays").Where(w => w.WeekId == weeknumber).FirstOrDefault();
                if (week == null)
                {
                    week = initNewWeek(weeknumber, context);
                    context.Weeks.Add(week);
                    context.SaveChanges();

                }
            }
            
            ViewBag.Title = "Uge " + weeknumber;
            return View("Week", week);
        }

        [HttpGet]
        public ActionResult Day(int weekNumber, int dayIndex)
        {
            Day day;
            int dayId = generateDayId(weekNumber, dayIndex);
            using(var context = new LunchClubContext())
            {
                // eager loading of Participants collection property
                day = context.Days.Include("Participants").Where(d => d.DayId == dayId).FirstOrDefault();
            }
            ViewBag.Title = WEEK_DAYS[dayIndex] + ", Uge " + weekNumber;
            return View("Day", day);
            
        }

        [HttpPost]
        public ActionResult Day(int weekNumber, int dayIndex, string name)
        {
            ActionResult result;
            Day day;
            int dayId = generateDayId(weekNumber, dayIndex);
            using (var context = new LunchClubContext())
            {
                // eager loading of Participants collection property
                day = context.Days.Include("Participants").Where(d => d.DayId == dayId).FirstOrDefault();
                Participant participant = day.Participants.Where(p => p.Name == name).FirstOrDefault();
                if (participant == null)
                {
                    participant = context.Participants.Where(p => p.Name == name).FirstOrDefault();
                    if(participant == null)
                    {
                        // no such participant exists
                        result = new HttpStatusCodeResult(202, "Medlemmet " + name + " findes ikke");
                    }
                    else
                    {
                        day.Participants.Add(participant);
                        context.SaveChanges();
                        result = View("Day", day);
                    }
                }else
                {
                    result = new HttpStatusCodeResult(202, "Medlemmet deltager allerede i denne frokost aftale");
                }
            }
            ViewBag.Title = WEEK_DAYS[dayIndex] + ", Uge " + weekNumber;
            return result;
        }

        public ActionResult RemoveDay(int dayId, string participantId)
        {
            Day day;
            Guid guid = new Guid(participantId);
            using (var context = new LunchClubContext())
            {
                // eager loading of Participants collection property
                day = context.Days.Include("Participants").Where(d => d.DayId == dayId).FirstOrDefault();
                day.Participants.RemoveAll(p => p.Id == guid);
                context.SaveChanges();
            }
            return View("Day", day);
        }
            

        private Week initNewWeek(int weekNumber, LunchClubContext context)
        {
            Week week = new Week();
            week.WeekId = weekNumber;
            week.WeekDays = new List<Day>();
            List<Participant> all_participants = context.Participants.ToList();
            for(var i = 0; i < WEEK_DAYS.Length; i++)
            {
                Day day = new Day();
                day.DayName = WEEK_DAYS[i];
                day.DayId = generateDayId(weekNumber, i);
                
                day.Participants = all_participants;
                week.WeekDays.Add(day);
            }
            return week;
        }

        private int generateDayId(int weekNumber, int dayIndex)
        {
            return (weekNumber * 10) + dayIndex;
        }
    }
}