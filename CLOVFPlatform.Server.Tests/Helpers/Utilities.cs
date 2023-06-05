using CLOVFPlatform.Server.Models;

namespace CLOVFPlatform.Server.Tests.Helpers
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(CLOVFContext db)
        {
            db.Employee.AddRange(GetSeedingEmployees());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(CLOVFContext db)
        {
            db.Employee.RemoveRange(db.Employee);
            InitializeDbForTests(db);
        }

        public static List<Employee> GetSeedingEmployees()
        {
            return new List<Employee>
            {
               new Employee { Id = Guid.NewGuid().ToString(), Name = "김민수", Email = "kimmins@gmail.com", Tel = "01012345678", Joined = DateTime.Parse("2020-03-15") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "이지현", Email = "leejihyun@gmail.com", Tel = "01023456789", Joined = DateTime.Parse("2019-11-28") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "박준호", Email = "parkjunho@gmail.com", Tel = "01034567890", Joined = DateTime.Parse("2021-07-10") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "최은지", Email = "choeunji@gmail.com", Tel = "01045678901", Joined = DateTime.Parse("2022-02-19") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "정민성", Email = "jeongminsung@gmail.com", Tel = "01056789012", Joined = DateTime.Parse("2020-12-05") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "강서윤", Email = "kangseoyun@gmail.com", Tel = "01067890123", Joined = DateTime.Parse("2019-08-23") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "조영민", Email = "joyoungmin@gmail.com", Tel = "01078901234", Joined = DateTime.Parse("2021-05-07") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "윤서진", Email = "yunseojin@gmail.com", Tel = "01089012345", Joined = DateTime.Parse("2022-08-14") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "장우진", Email = "jangwoojin@gmail.com", Tel = "01090123456", Joined = DateTime.Parse("2020-02-02") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "임지우", Email = "imjiwoo@gmail.com", Tel = "01001234567", Joined = DateTime.Parse("2019-10-17") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "한수진", Email = "hansujin@gmail.com", Tel = "01012345678", Joined = DateTime.Parse("2021-09-30") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "오태현", Email = "otaehyun@gmail.com", Tel = "01023456789", Joined = DateTime.Parse("2020-06-25") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "서영희", Email = "seoyounghi@gmail.com", Tel = "01034567890", Joined =DateTime.Parse( "2019-12-12") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "신동민", Email = "shindongmin@gmail.com", Tel = "01045678901", Joined = DateTime.Parse("2022-03-05") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "권미란", Email = "kwonmiran@gmail.com", Tel = "01056789012", Joined = DateTime.Parse("2020-01-30")},
                new Employee { Id =Guid.NewGuid().ToString(), Name = "황선영", Email = "hwangseonyoung@gmail.com", Tel = "01067890123", Joined = DateTime.Parse("2019-07-14") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "안현우", Email = "anhyunwoo@gmail.com", Tel = "01078901234", Joined = DateTime.Parse("2021-04-28") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "송영철", Email = "songyeongcheol@gmail.com", Tel = "01089012345", Joined = DateTime.Parse("2022-07-07") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "류준서", Email = "ryujunseo@gmail.com", Tel = "01090123456", Joined = DateTime.Parse("2020-08-01") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "홍미경", Email = "hongmikyung@gmail.com", Tel = "01001234567", Joined = DateTime.Parse("2019-11-11") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "아무개", Email = "anonymous1@gmail.com", Tel = "01056789876", Joined = DateTime.Parse("2019-11-11") },
                new Employee { Id =Guid.NewGuid().ToString(), Name = "아무개", Email = "anonymous1@gmail.com", Tel = "01047859325", Joined = DateTime.Parse("2012-01-27") },
            };
        }
        #endregion
    }
}
