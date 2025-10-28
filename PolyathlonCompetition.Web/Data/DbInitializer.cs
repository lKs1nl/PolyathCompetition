using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PolyathlonCompetition.Web.Models;

namespace PolyathlonCompetition.Web.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            // Создание ролей
            string[] roleNames = { "Admin", "Organizer", "Judge" };
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Создание администратора по умолчанию
            var adminUser = await userManager.FindByEmailAsync("admin@polyathlon.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@polyathlon.com",
                    Email = "admin@polyathlon.com",
                    FullName = "Администратор Системы",
                    RoleType = "Admin",
                    EmailConfirmed = true
                };

                var createPowerUser = await userManager.CreateAsync(user, "Admin123!");
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Создание организатора
            var organizerUser = await userManager.FindByEmailAsync("organizer@polyathlon.com");
            if (organizerUser == null)
            {
                var organizer = new ApplicationUser
                {
                    UserName = "organizer@polyathlon.com",
                    Email = "organizer@polyathlon.com",
                    FullName = "Организатор Соревнований",
                    RoleType = "Organizer",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(organizer, "Organizer123!");
                await userManager.AddToRoleAsync(organizer, "Organizer");
            }

            // Создание судьи
            var judgeUser = await userManager.FindByEmailAsync("judge@polyathlon.com");
            if (judgeUser == null)
            {
                var judge = new ApplicationUser
                {
                    UserName = "judge@polyathlon.com",
                    Email = "judge@polyathlon.com",
                    FullName = "Судья Международной Категории",
                    RoleType = "Judge",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(judge, "Judge123!");
                await userManager.AddToRoleAsync(judge, "Judge");
            }

            // Создание тестовых данных для соревнований
            if (!context.Competitions.Any())
            {
                var competitions = new List<Competition>
                {
                    new Competition
                    {
                        Name = "Чемпионат мира по полиатлону 2024",
                        Date = DateTime.Now.AddDays(30),
                        Location = "Международный спортивный комплекс, Москва",
                        Status = CompetitionStatus.RegistrationOpen,
                        MaxParticipants = 200,
                        Description = "Ежегодный чемпионат мира по полиатлону с участием лучших спортсменов со всего мира."
                    },
                    new Competition
                    {
                        Name = "Кубок Европы по полиатлону",
                        Date = DateTime.Now.AddDays(60),
                        Location = "Европейский спортивный центр, Берлин",
                        Status = CompetitionStatus.Planned,
                        MaxParticipants = 150,
                        Description = "Престижный кубок Европы с квалификацией на чемпионат мира."
                    },
                    new Competition
                    {
                        Name = "Международный турнир 'Золотой Мультиспорт'",
                        Date = DateTime.Now.AddDays(15),
                        Location = "Олимпийский стадион, Сочи",
                        Status = CompetitionStatus.Ongoing,
                        MaxParticipants = 100,
                        Description = "Традиционный международный турнир с богатой историей."
                    },
                    new Competition
                    {
                        Name = "Национальный чемпионат по летнему полиатлону",
                        Date = DateTime.Now.AddDays(-30),
                        Location = "Центральный стадион, Санкт-Петербург",
                        Status = CompetitionStatus.Completed,
                        MaxParticipants = 120,
                        Description = "Ежегодный национальный чемпионат, завершившийся неделю назад."
                    }
                };

                context.Competitions.AddRange(competitions);
                await context.SaveChangesAsync();
            }

            // Создание тестовых данных для участников
            if (!context.Competitors.Any())
            {
                var competitors = new List<Competitor>
                {
                    new Competitor
                    {
                        FirstName = "Иван",
                        LastName = "Петров",
                        MiddleName = "Сергеевич",
                        BirthDate = new DateTime(1995, 5, 15),
                        Team = "Сборная России",
                        Category = "Мужчины",
                        CompetitorNumber = "P001"
                    },
                    new Competitor
                    {
                        FirstName = "Анна",
                        LastName = "Сидорова",
                        MiddleName = "Ивановна",
                        BirthDate = new DateTime(1998, 8, 22),
                        Team = "Сборная России",
                        Category = "Женщины",
                        CompetitorNumber = "P002"
                    },
                    new Competitor
                    {
                        FirstName = "Михаил",
                        LastName = "Козлов",
                        MiddleName = "Александрович",
                        BirthDate = new DateTime(1993, 3, 10),
                        Team = "ЦСКА",
                        Category = "Мужчины",
                        CompetitorNumber = "P003"
                    },
                    new Competitor
                    {
                        FirstName = "Елена",
                        LastName = "Новикова",
                        MiddleName = "Викторовна",
                        BirthDate = new DateTime(1997, 11, 5),
                        Team = "Динамо",
                        Category = "Женщины",
                        CompetitorNumber = "P004"
                    },
                    new Competitor
                    {
                        FirstName = "Алексей",
                        LastName = "Морозов",
                        MiddleName = "Дмитриевич",
                        BirthDate = new DateTime(1996, 7, 18),
                        Team = "Спартак",
                        Category = "Мужчины",
                        CompetitorNumber = "P005"
                    },
                    new Competitor
                    {
                        FirstName = "Ольга",
                        LastName = "Волкова",
                        MiddleName = "Петровна",
                        BirthDate = new DateTime(1999, 2, 28),
                        Team = "Локомотив",
                        Category = "Женщины",
                        CompetitorNumber = "P006"
                    },
                    new Competitor
                    {
                        FirstName = "Дмитрий",
                        LastName = "Орлов",
                        MiddleName = "Игоревич",
                        BirthDate = new DateTime(1994, 9, 12),
                        Team = "Сборная Беларуси",
                        Category = "Мужчины",
                        CompetitorNumber = "P007"
                    },
                    new Competitor
                    {
                        FirstName = "Мария",
                        LastName = "Лебедева",
                        MiddleName = "Сергеевна",
                        BirthDate = new DateTime(2000, 4, 3),
                        Team = "Сборная Украины",
                        Category = "Женщины",
                        CompetitorNumber = "P008"
                    }
                };

                context.Competitors.AddRange(competitors);
                await context.SaveChangesAsync();
            }

            // Создание тестовых регистраций на соревнования
            if (!context.CompetitionRegistrations.Any())
            {
                var championship = await context.Competitions
                    .FirstAsync(c => c.Name == "Чемпионат мира по полиатлону 2024");
                
                var europeCup = await context.Competitions
                    .FirstAsync(c => c.Name == "Кубок Европы по полиатлону");
                
                var competitors = await context.Competitors.ToListAsync();

                var registrations = new List<CompetitionRegistration>
                {
                    // Регистрации на Чемпионат мира
                    new CompetitionRegistration
                    {
                        CompetitionId = championship.Id,
                        CompetitorId = competitors[0].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-10),
                        Status = RegistrationStatus.Approved,
                        Notes = "Чемпион прошлого года"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = championship.Id,
                        CompetitorId = competitors[1].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-8),
                        Status = RegistrationStatus.Approved,
                        Notes = "Перспективный спортсмен"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = championship.Id,
                        CompetitorId = competitors[2].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-5),
                        Status = RegistrationStatus.Pending,
                        Notes = "Ожидает подтверждения документов"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = championship.Id,
                        CompetitorId = competitors[3].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-3),
                        Status = RegistrationStatus.Approved,
                        Notes = "Рекомендован тренерским штабом"
                    },

                    // Регистрации на Кубок Европы
                    new CompetitionRegistration
                    {
                        CompetitionId = europeCup.Id,
                        CompetitorId = competitors[4].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-7),
                        Status = RegistrationStatus.Approved,
                        Notes = "Участник квалификации"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = europeCup.Id,
                        CompetitorId = competitors[5].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-6),
                        Status = RegistrationStatus.Approved,
                        Notes = "Молодой спортсмен"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = europeCup.Id,
                        CompetitorId = competitors[6].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-4),
                        Status = RegistrationStatus.Rejected,
                        Notes = "Не прошел медицинский осмотр"
                    },
                    new CompetitionRegistration
                    {
                        CompetitionId = europeCup.Id,
                        CompetitorId = competitors[7].Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-2),
                        Status = RegistrationStatus.Pending,
                        Notes = "Ожидает визу"
                    }
                };

                context.CompetitionRegistrations.AddRange(registrations);
                await context.SaveChangesAsync();
            }

            // Создание тестовых результатов для завершенного соревнования
            if (!context.CompetitionResults.Any())
            {
                var completedCompetition = await context.Competitions
                    .FirstAsync(c => c.Status == CompetitionStatus.Completed);
                
                var approvedRegistrations = await context.CompetitionRegistrations
                    .Where(r => r.CompetitionId == completedCompetition.Id && r.Status == RegistrationStatus.Approved)
                    .Include(r => r.Competitor)
                    .ToListAsync();

                if (approvedRegistrations.Any())
                {
                    var results = new List<CompetitionResult>();
                    var place = 1;
                    
                    foreach (var registration in approvedRegistrations.Take(3))
                    {
                        results.Add(new CompetitionResult
                        {
                            CompetitionId = completedCompetition.Id,
                            CompetitorId = registration.CompetitorId,
                            RunningTime = 12.5 + (place * 0.3),
                            SwimmingTime = 45.2 + (place * 1.5),
                            ShootingScore = 95 - (place * 5),
                            StrengthScore = 85 - (place * 3),
                            TotalScore = 280 - (place * 10),
                            Place = place
                        });
                        place++;
                    }

                    context.CompetitionResults.AddRange(results);
                    await context.SaveChangesAsync();
                }
            }

            Console.WriteLine("База данных успешно инициализирована с тестовыми данными!");
            Console.WriteLine("Данные для входа:");
            Console.WriteLine("Администратор: admin@polyathlon.com / Admin123!");
            Console.WriteLine("Организатор: organizer@polyathlon.com / Organizer123!");
            Console.WriteLine("Судья: judge@polyathlon.com / Judge123!");
        }
    }
}