using System;

namespace CLOVFPlatform.Server.Services.DTO
{
    /// <summary>
    /// 직원 정보
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 키
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 연락처
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 입사일
        /// </summary>
        public DateTime? Joined { get; set; }

        public Employee()
        {
        }
    }
}

