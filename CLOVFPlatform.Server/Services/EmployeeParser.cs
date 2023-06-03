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
    

    public class EmployeeFileParser : IEmployeeFileParser
    {
        public bool IsValid(string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmployeeDTO> GetEmployee(string value)
        {
            throw new NotImplementedException();
        }
    }

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
                if (IsValid(value))
                {
                    var token = JToken.Parse(value);
                    if (token is JArray)
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<EmployeeDTO>>(value)!;
                    }

                    return new EmployeeDTO[] { JsonConvert.DeserializeObject<EmployeeDTO>(value)! };
                }

                return Array.Empty<EmployeeDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

	public class EmployeeCsvParser : IEmployeeCsvParser
	{
        public bool IsValid(string value)
        {
            return true;
        }

        public IEnumerable<EmployeeDTO> GetEmployee(string value)
        {
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(value);
                using var ms = new MemoryStream(bytes);
                using var sr = new StreamReader(ms);
                using var csv = new CsvReader(sr, System.Globalization.CultureInfo.InvariantCulture);
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
