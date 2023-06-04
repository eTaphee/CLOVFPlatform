using System;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;

namespace CLOVFPlatform.Server.Services
{
	public interface IEmployeeParser
	{
		bool IsValid(string value);
		IEnumerable<EmployeeDTO> GetEmployee(string value);
	}

    public interface IEmployeeFileParser : IEmployeeParser { }
    public interface IEmployeeJsonParser : IEmployeeParser { }
    public interface IEmployeeCsvParser : IEmployeeParser { }
    
    public class EmployeeJsonParser : IEmployeeJsonParser
    {
        public bool IsValid(string value)
        {
            try
            {
                JToken.Parse(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<EmployeeDTO> GetEmployee(string value)
        {
            try
            {
                if (!IsValid(value))
                {
                    return Array.Empty<EmployeeDTO>();
                }

                var token = JToken.Parse(value);

                if (token is JArray)
                {
                    return token.ToObject<IEnumerable<EmployeeDTO>>()!;
                }

                return new EmployeeDTO[] { token.ToObject<EmployeeDTO>()! };
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

	public class EmployeeCsvParser : IEmployeeCsvParser
	{
        private CsvReader GetCsvReader(string csvString)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvString);
            var ms = new MemoryStream(bytes);
            var sr = new StreamReader(ms);
            return new CsvReader(sr, System.Globalization.CultureInfo.InvariantCulture);
        }

        private bool IsValid(CsvReader reader)
        {
            return true;
        }

        public bool IsValid(string value)
        {
            using var csv = GetCsvReader(value);
            return IsValid(csv);
        }

        public IEnumerable<EmployeeDTO> GetEmployee(string value)
        {
            try
            {
                using var csv = GetCsvReader(value);
                if (!IsValid(csv))
                {
                    return Array.Empty<EmployeeDTO>();
                }

                var list = new List<EmployeeDTO>();

                while (csv.Read())
                {
                    list.Add(new EmployeeDTO
                    {
                        Name = csv.GetField<string>(0)!.Trim(),
                        Email = csv.GetField<string>(1)!.Trim(),
                        Tel = csv.GetField<string>(2)!.Trim(),
                        Joined = csv.GetField<DateTime>(3)!
                    });
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
