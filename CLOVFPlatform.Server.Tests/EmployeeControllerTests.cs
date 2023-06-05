using CLOVFPlatform.Server.Services.DTO;
using CLOVFPlatform.Server.Tests.Helpers;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using PaginatedList = CLOVFPlatform.Server.Services.DTO.PaginatedList<CLOVFPlatform.Server.Services.DTO.Employee>;

namespace CLOVFPlatform.Server.Tests
{
    [TestClass]
    public class EmployeeControllerTests
    {
        static TestWebApplicationFactory factory;
        const string FILE_VALID_JSON_ARRAY = "./Samples/employee-array.json";
        const string FILE_VALID_JSON_DUPLICATED_ARRAY = "./Samples/employee-array-dup.json";
        const string FILE_VALID_JSON_DUPLICATED2_ARRAY = "./Samples/employee-array-dup2.json";
        const string FILE_VALID_JSON_OBJECT = "./Samples/employee-one.json";
        const string FILE_INVALID_JSON_ARRAY = "./Samples/employee-array-invalid.json";

        const string FILE_VALID_CSV_ARRAY = "./Samples/employee-array.csv";
        const string FILE_VALID_CSV_DUPLICATED_ARRAY = "./Samples/employee-array-dup.csv";
        const string FILE_VALID_CSV_DUPLICATED2_ARRAY = "./Samples/employee-array-dup2.csv";
        const string FILE_VALID_CSV_OBJECT = "./Samples/employee-one.csv";


        [TestInitialize]
        public void Initalize()
        {
            factory= new TestWebApplicationFactory();
        }

        #region GET /api/employee
        /// <summary>
        /// ���� ���� ��ȸ
        /// </summary>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(1, 10)]
        [DataRow(2, 10)]
        [DataRow(1, 5)]
        [DataRow(30, 5)]
        public async Task Test_GetEmployees(int page, int pageSize)
        {
            // action
            var client = factory.CreateClient();
            var response = await client.GetAsync($"/api/employee?page={page}&pageSize={pageSize}");
            var result = await ToObjectAsync<PaginatedList>(response);
            var list = Utilities.GetSeedingEmployees();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result.Page, page);
            Assert.AreEqual(result.PageSize, pageSize);
            Assert.AreEqual(result.TotalCount, list.Count);
        }

        /// <summary>
        /// �̸����� ���� ���� ��ȸ(����)
        /// </summary>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(1, 10, "��μ�")]
        [DataRow(1, 10, "������")]
        [DataRow(1, 10, "����ȣ")]
        [DataRow(1, 10, "������")]
        public async Task Test_GetEmployeesByName_ShouldReturnOK(int page, int pageSize, string name)
        {
            // action
            var client = factory.CreateClient();
            var response = await client.GetAsync($"/api/employee/{name}?page={page}&pageSize={pageSize}");
            var result = await ToObjectAsync<PaginatedList>(response); 
            var list = Utilities.GetSeedingEmployees().Where(m => m.Name == name).ToList();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result.Page, page);
            Assert.AreEqual(result.PageSize, pageSize);
            Assert.AreEqual(result.TotalCount, list.Count);
        }

        /// <summary>
        /// �̸����� ���� ���� ��ȸ(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_GetEmployeesByName_ShouldReturnOK_WithZeroItems()
        {
            // arrange
            int page = 1;
            int pageSize = 10;
            string name = "�ڹ����";

            // action
            var client = factory.CreateClient();
            var response = await client.GetAsync($"/api/employee/{name}?page={page}&pageSize={pageSize}");
            var result = await ToObjectAsync<PaginatedList>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(result.TotalCount, 0);
            Assert.AreEqual(result.Items!.Count(), 0);
        }
        #endregion

        #region POST /api/employee �׽�Ʈ ���� �Ű�����
        /// <summary>
        ///  ���Ϸ� ���� ����(�迭) �߰�(����)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_ARRAY)]
        [DataRow(FILE_VALID_CSV_ARRAY)]
        public async Task Test_CreateEmployee_WithArrayDataFile_ShouldReturnCreated(string file)
        {
            // arange
            var formData = CreateFileFormData(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// ���Ϸ� ���� ����(�迭) �߰�(����), ��� �ߺ��� ������
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_ARRAY)]
        [DataRow(FILE_VALID_CSV_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataFile_WhenAllDataDuplicated_ShouldReturnNoContent(string file)
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", formData);
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// ���Ϸ� ���� ����(�迭) �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_DUPLICATED_ARRAY)]
        [DataRow(FILE_VALID_CSV_DUPLICATED_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataFile_WhenSomeDataDuplicated_ShouldReturnCreated(string file)
        {
            // arange
            var formData = CreateFileFormData(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// ���Ϸ� ���� ����(�迭) �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_DUPLICATED2_ARRAY)]
        [DataRow(FILE_VALID_CSV_DUPLICATED2_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataFile_WhenOneDataCreated_ShouldReturnCreated(string file)
        {
            // arange
            var formData = CreateFileFormData(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// ���Ϸ� ���� ���� �߰�(����)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_OBJECT)]
        [DataRow(FILE_VALID_JSON_OBJECT)]
        public async Task Test_CreateEmpolyee_WithDataFile_ShouldReturnCreated(string file)
        {
            // arange
            var formData = CreateFileFormData(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// ���ڿ��� ���� ����(�迭) �߰�(����), 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_ARRAY)]
        [DataRow(FILE_VALID_CSV_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataString_ShouldReturnCreated(string file)
        {
            // arange
            var content = CreateStringContentFromFile(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// ���ڿ��� ���� ����(�迭) �߰�(����),, ��� �ߺ��� ������
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_ARRAY)]
        [DataRow(FILE_VALID_CSV_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataString_WhenAllDataDuplicated_ShouldReturnNoContent(string file)
        {
            // arange
            var content = CreateStringContentFromFile(file);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", content);
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// ���ڿ��� ���� ����(�迭) �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_DUPLICATED_ARRAY)]
        [DataRow(FILE_VALID_CSV_DUPLICATED_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataString_WhenSomeDataDuplicated_ShouldReturnCreated(string file)
        {
            // arange
            var content = CreateStringContentFromFile(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// ���ڿ��� ���� ����(�迭) �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_DUPLICATED2_ARRAY)]
        [DataRow(FILE_VALID_CSV_DUPLICATED2_ARRAY)]
        public async Task Test_CreateEmpolyee_WithArrayDataString_WhenOneDataCreated_ShouldReturnCreated(string file)
        {
            // arange
            var content = CreateStringContentFromFile(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// ���ڿ��� ���� ���� �߰�(����)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DataTestMethod]
        [DataRow(FILE_VALID_JSON_OBJECT)]
        [DataRow(FILE_VALID_JSON_OBJECT)]
        public async Task Test_CreateEmpolyee_WithDataString_ShouldReturnCreated(string file)
        {
            // arange
            var formData = CreateFileFormData(file);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }
        #endregion

        #region POST /api/employee json
        /// <summary>
        /// json ����(���� �迭)�� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonFile_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// json ����(���� �迭)�� ���� ���� �߰�(����), ��� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonFile_WhenAllDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", formData);
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// json ����(���� �迭)�� ���� ���� �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonFile_WhenSomeDataDuplicated_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_DUPLICATED_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// json ����(���� �迭)�� ���� ���� �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonFile_WhenOneDataCreated_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_DUPLICATED2_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// json string(���� �迭)�� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonString_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// json string(���� �迭)�� ���� ���� �߰�(����), ��� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonString_WhenAllDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", content);
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// json string(���� �迭)�� ���� ���� �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonString_WhenSomeDataDuplicated_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_DUPLICATED_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// json string(���� �迭)�� ���� ���� �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayJsonString_WhenOneDataCreated_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_DUPLICATED2_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// json ���Ϸ� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectJsonFile_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_OBJECT);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// json ���Ϸ� ���� ���� �߰�(����), �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectJsonFile_WhenDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_OBJECT);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", formData);
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// json string ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectJsonString_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_OBJECT);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// json string ���� ���� �߰�(����), �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectJsonString_WhenDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_JSON_OBJECT);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", content);
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// ��ȿ���� ���� json ���Ϸ� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithInvalidJsonFile_ShouldReturnBadRequest()
        {
            // arange
            var formData = new MultipartFormDataContent
            {
                { new StreamContent(File.OpenRead(FILE_INVALID_JSON_ARRAY)), "file", FILE_INVALID_JSON_ARRAY }
            };

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
        #endregion

        #region POST /api/employee csv

        /// <summary>
        /// csv ����(���� �迭)�� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvFile_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_JSON_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// csv ����(���� �迭)�� ���� ���� �߰�(����), ��� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvFile_WhenAllDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_CSV_ARRAY);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", formData);
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// csv ����(���� �迭)�� ���� ���� �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvFile_WhenSomeDataDuplicated_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_CSV_DUPLICATED_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// csv ����(���� �迭)�� ���� ���� �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvFile_WhenOneDataCreated_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_CSV_DUPLICATED2_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// csv string(���� �迭)�� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvString_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// csv string(���� �迭)�� ���� ���� �߰�(����), ��� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvString_WhenAllDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_ARRAY);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", content);
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// csv string(���� �迭)�� ���� ���� �߰�(����), �Ϻ� �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvString_WhenSomeDataDuplicated_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_DUPLICATED_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<IEnumerable<Employee>>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// csv string(���� �迭)�� ���� ���� �߰�(����), �ϳ��� �߰����� ��
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectArrayCsvString_WhenOneDataCreated_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_DUPLICATED2_ARRAY);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// csv ���Ϸ� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectCsvFile_ShouldReturnCreated()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_CSV_OBJECT);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// csv ���Ϸ� ���� ���� �߰�(����), �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectCsvFile_WhenDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var formData = CreateFileFormData(FILE_VALID_CSV_OBJECT);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", formData);
            var response = await client.PostAsync("/api/employee", formData);
            var result = await ToObjectAsync<Employee>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// csv string ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectCsvString_ShouldReturnCreated()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_OBJECT);

            // action
            var client = factory.CreateClient();
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);
            var location = response.Headers.GetValues("location").FirstOrDefault();

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(result);
            Assert.AreEqual($"/api/employee/{result.Id}", location);
        }

        /// <summary>
        /// csv string ���� ���� �߰�(����), �ߺ��� ������
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateEmpolyee_WithObjectCsvString_WhenDataDuplicated_ShouldReturnNoContent()
        {
            // arange
            var content = CreateStringContentFromFile(FILE_VALID_CSV_OBJECT);

            // action
            var client = factory.CreateClient();
            await client.PostAsync("/api/employee", content);
            var response = await client.PostAsync("/api/employee", content);
            var result = await ToObjectAsync<Employee>(response);

            // assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
            Assert.IsNull(result);
        }

        /// <summary>
        /// ��ȿ���� ���� json ���Ϸ� ���� ���� �߰�(����)
        /// </summary>
        /// <returns></returns>
        //[TestMethod]
        //public async Task Test_CreateEmpolyee_WithInvalidJsonFile_ShouldReturnBadRequest()
        //{
        //    // arange
        //    var formData = new MultipartFormDataContent
        //    {
        //        { new StreamContent(File.OpenRead(FILE_INVALID_JSON_ARRAY)), "file", FILE_INVALID_JSON_ARRAY }
        //    };

        //    // action
        //    var client = factory.CreateClient();
        //    var response = await client.PostAsync("/api/employee", formData);

        //    // assert
        //    Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        //}
        #endregion

        StringContent CreateStringContentFromFile(string file)
        {
            var ext = Path.GetExtension(file.ToLower());

            var content = File.ReadAllText(file);

            var mediaType = ext.Equals(".json") ? "application/json" : "text/csv";

            return new StringContent(content, Encoding.UTF8, mediaType);
        }

        MultipartFormDataContent CreateFileFormData(string file)
        {
            return new MultipartFormDataContent
            {
                { new StreamContent(File.OpenRead(file)), "file", file }
            };
        }

        async Task<T> ToObjectAsync<T>(HttpResponseMessage response)
        {
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}