using VirtualEventScheduler.Data;
using VirtualEventScheduler.Data.Models;

namespace VirtualEventScheduler.API
{
    /// <summary>
    /// Seeds realistic demo data on first run so the app looks good during presentations.
    /// Checks whether data already exists before inserting — safe to call on every startup.
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            // Only seed if there are no events yet (idempotent)
            if (db.Events.Any())
                return;

            // ── Seed demo users ────────────────────────────────────────────
            // Password for all demo users: Demo1234!
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234!", 10);

            var staff = new User
            {
                FullName     = "Serhat Uzunbayır",
                Email        = "serhat@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Staff",
                CreatedAt    = new DateTime(2026, 1, 10)
            };

            var attendee1 = new User
            {
                FullName     = "Mehmet Taşoğlu",
                Email        = "mehmet@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Attendee",
                CreatedAt    = new DateTime(2026, 1, 15)
            };

            var attendee2 = new User
            {
                FullName     = "Öykü İrem Oplazgıl",
                Email        = "oyku@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Attendee",
                CreatedAt    = new DateTime(2026, 1, 20)
            };

            var attendee3 = new User
            {
                FullName     = "Batuhan Bulut",
                Email        = "batuhan@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Attendee",
                CreatedAt    = new DateTime(2026, 2, 1)
            };

            var attendee4 = new User
            {
                FullName     = "Zeynep Kaya",
                Email        = "zeynep@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Attendee",
                CreatedAt    = new DateTime(2026, 2, 5)
            };

            var attendee5 = new User
            {
                FullName     = "Ali Yılmaz",
                Email        = "ali@eventapp.com",
                PasswordHash = passwordHash,
                Role         = "Attendee",
                CreatedAt    = new DateTime(2026, 2, 10)
            };

            db.Users.AddRange(staff, attendee1, attendee2, attendee3, attendee4, attendee5);
            db.SaveChanges();

            // ── Seed events ────────────────────────────────────────────────
            var events = new List<Event>
            {
                // 1 — Nearly full → red progress bar (5/6 = 83%)
                new Event
                {
                    Title       = "Introduction to Cloud Computing",
                    Description = "A comprehensive workshop covering AWS, Azure, and GCP fundamentals. Learn how to deploy scalable applications and manage cloud infrastructure in a hands-on environment.",
                    DateTime    = new DateTime(2026, 6, 10, 14, 0, 0),
                    Location    = "Zoom Meeting — Room A",
                    Capacity    = 6,
                    CreatedBy   = 1, // admin
                    Status      = "Active",
                    CreatedAt   = new DateTime(2026, 5, 1)
                },
                // 2 — Half full → yellow bar (3/5 = 60%)
                new Event
                {
                    Title       = "Web Development with ASP.NET Core",
                    Description = "Deep dive into building modern web applications using ASP.NET Core MVC, Entity Framework Core, and RESTful API design patterns. Suitable for intermediate developers.",
                    DateTime    = new DateTime(2026, 6, 15, 10, 0, 0),
                    Location    = "Microsoft Teams — Channel SE410",
                    Capacity    = 5,
                    CreatedBy   = staff.Id,
                    Status      = "Active",
                    CreatedAt   = new DateTime(2026, 5, 5)
                },
                // 3 — Mostly empty, green bar
                new Event
                {
                    Title       = "Mobile App Development with .NET MAUI",
                    Description = "Explore cross-platform mobile development using .NET MAUI. Build apps that run on iOS, Android, macOS, and Windows from a single codebase.",
                    DateTime    = new DateTime(2026, 6, 20, 13, 0, 0),
                    Location    = "Google Meet — Project Room",
                    Capacity    = 50,
                    CreatedBy   = 1,
                    Status      = "Active",
                    CreatedAt   = new DateTime(2026, 5, 10)
                },
                // 4 — FULL → Full badge (6/6 = 100%)
                new Event
                {
                    Title       = "Database Design & SQL Best Practices",
                    Description = "Learn advanced SQL techniques, normalization, indexing strategies, and query optimization. Practical examples using SQL Server and SQLite.",
                    DateTime    = new DateTime(2026, 6, 25, 11, 0, 0),
                    Location    = "Engineering Building — Lab 3",
                    Capacity    = 6,
                    CreatedBy   = staff.Id,
                    Status      = "Active",
                    CreatedAt   = new DateTime(2026, 5, 12)
                },
                // 5 — Completed past event
                new Event
                {
                    Title       = "Agile Project Management Seminar",
                    Description = "An overview of Agile methodologies including Scrum, Kanban, and SAFe. Includes real-world case studies from software development teams.",
                    DateTime    = new DateTime(2026, 5, 20, 15, 0, 0),
                    Location    = "Online — Webex",
                    Capacity    = 60,
                    CreatedBy   = 1,
                    Status      = "Completed",
                    CreatedAt   = new DateTime(2026, 4, 20)
                },
                // 6 — Cancelled event
                new Event
                {
                    Title       = "Python for Data Science",
                    Description = "Introduction to Python libraries such as NumPy, Pandas, and Matplotlib for data analysis and visualisation. Prerequisites: basic programming knowledge.",
                    DateTime    = new DateTime(2026, 6, 5, 9, 0, 0),
                    Location    = "Zoom Meeting — Room B",
                    Capacity    = 35,
                    CreatedBy   = staff.Id,
                    Status      = "Cancelled",
                    CreatedAt   = new DateTime(2026, 5, 1)
                },
                // 7 — Coming up very soon (good for reminder demo)
                new Event
                {
                    Title       = "SE 410 Final Project Presentations",
                    Description = "Student teams present their semester projects for SE 410 Software Framework Applications. Attendance is mandatory for all enrolled students.",
                    DateTime    = new DateTime(2026, 6, 4, 10, 20, 0),
                    Location    = "Engineering Building — Auditorium",
                    Capacity    = 80,
                    CreatedBy   = 1,
                    Status      = "Active",
                    CreatedAt   = new DateTime(2026, 5, 13)
                }
            };

            db.Events.AddRange(events);
            db.SaveChanges();

            // ── Seed registrations ─────────────────────────────────────────
            var registrations = new List<EventRegistration>
            {
                // Cloud Computing (capacity 30) — 27 registered → nearly full (red bar)
                new EventRegistration { EventId = events[0].Id, UserId = attendee1.Id, RegisteredAt = new DateTime(2026, 5, 10) },
                new EventRegistration { EventId = events[0].Id, UserId = attendee2.Id, RegisteredAt = new DateTime(2026, 5, 11) },
                new EventRegistration { EventId = events[0].Id, UserId = attendee3.Id, RegisteredAt = new DateTime(2026, 5, 12) },
                new EventRegistration { EventId = events[0].Id, UserId = attendee4.Id, RegisteredAt = new DateTime(2026, 5, 13) },
                new EventRegistration { EventId = events[0].Id, UserId = attendee5.Id, RegisteredAt = new DateTime(2026, 5, 14) },

                // ASP.NET Core event (capacity 40) — 20 registered → half full (yellow bar)
                new EventRegistration { EventId = events[1].Id, UserId = attendee1.Id, RegisteredAt = new DateTime(2026, 5, 15) },
                new EventRegistration { EventId = events[1].Id, UserId = attendee3.Id, RegisteredAt = new DateTime(2026, 5, 16) },
                new EventRegistration { EventId = events[1].Id, UserId = attendee5.Id, RegisteredAt = new DateTime(2026, 5, 17) },

                // MAUI event (capacity 50) — 5 registered → mostly empty (green bar)
                new EventRegistration { EventId = events[2].Id, UserId = attendee2.Id, RegisteredAt = new DateTime(2026, 5, 18) },
                new EventRegistration { EventId = events[2].Id, UserId = attendee4.Id, RegisteredAt = new DateTime(2026, 5, 19) },

                // Database event (capacity 20) — 20 registered → FULL
                new EventRegistration { EventId = events[3].Id, UserId = attendee1.Id, RegisteredAt = new DateTime(2026, 5, 13) },
                new EventRegistration { EventId = events[3].Id, UserId = attendee2.Id, RegisteredAt = new DateTime(2026, 5, 13) },
                new EventRegistration { EventId = events[3].Id, UserId = attendee3.Id, RegisteredAt = new DateTime(2026, 5, 14) },
                new EventRegistration { EventId = events[3].Id, UserId = attendee4.Id, RegisteredAt = new DateTime(2026, 5, 14) },
                new EventRegistration { EventId = events[3].Id, UserId = attendee5.Id, RegisteredAt = new DateTime(2026, 5, 14) },
                new EventRegistration { EventId = events[3].Id, UserId = staff.Id,     RegisteredAt = new DateTime(2026, 5, 15) },

                // SE 410 Presentations (upcoming soon — good for reminder demo)
                new EventRegistration { EventId = events[6].Id, UserId = attendee1.Id, RegisteredAt = new DateTime(2026, 5, 20) },
                new EventRegistration { EventId = events[6].Id, UserId = attendee2.Id, RegisteredAt = new DateTime(2026, 5, 20) },
                new EventRegistration { EventId = events[6].Id, UserId = attendee3.Id, RegisteredAt = new DateTime(2026, 5, 21) },
                new EventRegistration { EventId = events[6].Id, UserId = attendee4.Id, RegisteredAt = new DateTime(2026, 5, 21) },
                new EventRegistration { EventId = events[6].Id, UserId = attendee5.Id, RegisteredAt = new DateTime(2026, 5, 22) },
            };

            db.EventRegistrations.AddRange(registrations);
            db.SaveChanges();
        }
    }
}
