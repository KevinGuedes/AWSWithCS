using Customers.Repository.Contracts.Data;
using Customers.Repository.Repositories;

var cts = new CancellationTokenSource();
var customerRepository = new CustomerRepository();

var customerDto = new CustomerDto
{
    DateOfBirth = DateTime.Now,
    Email = "fake@gmail.com",
    FullName = "User Fake",
    GitHubUsername = "fakeuser",
    Id = Guid.NewGuid(),
};

try
{
    var cancellationToken = cts.Token;

    //await customerRepository.CreateAsync(customerDto, cancellationToken);
    //var customerFromDb = await customerRepository.GetAsync(customerDto.Id, cancellationToken);
    //var deleteResponse = await customerRepository.DeleteAsync(Guid.Parse("021dc6be-6a66-4c56-bac5-f521fa52c438"), cancellationToken);
    //var updateResponse = await customerRepository.UpdateAsync(customerDto, DateTime.UtcNow, cancellationToken);

    //var customers = new List<CustomerDto> { customerDto, customerDto2 };
    //var createInBatch = await customerRepository.CreateCustomersInBatchAsync(customers, cancellationToken);
    var result = await customerRepository.GetByEmailAsync("fake@gmail.com", cancellationToken);
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
