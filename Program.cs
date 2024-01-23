using HoneyRaesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

List<Customer> customers = new List<Customer> 
{
    new Customer()
    {
        Id = 24,
        Name = "Nik Lizcano",
        Address = "1234 Sunset Place, Nashville TN 12345"
    }, 
    new Customer()
    {
        Id = 54,
        Name = "Jason Peterson",
        Address = "4567 Ordway Place, Nashville TN 23456"
    },
    new Customer()
    {
        Id = 33,
        Name = "Laura Epling",
        Address = "3276 Crystal Oak Drive, Nashville TN 32678"
    }
};
List<Employee> employees = new List<Employee> 
{
    new Employee()
    {
        Id = 87,
        Name = "Micaela Miller",
        Specialty = "Puzzles and Overthinking"
    }, 
    new Employee()
    {
        Id = 77,
        Name = "Natalie Mays",
        Specialty = "Crafts and Random Facts"
    }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 54,
        EmployeeId = 87,
        Description = "Description for ticket number 1",
        Emergency = true,
        DateCompleted = null,
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 33,
        EmployeeId = 77,
        Description = "Description for ticket number 2.",
        Emergency = false,
        DateCompleted = null,
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 33,
        EmployeeId = 87,
        Description = "Description for ticket number 3.",
        Emergency = false,
        DateCompleted = new DateTime(2024, 1, 10),
    },
     new ServiceTicket()
    {
        Id = 4,
        CustomerId = 24,
        EmployeeId = 0,
        Description = "Description for ticket number 4.",
        Emergency = true,
        DateCompleted = new DateTime(2023, 11, 10),
    },
     new ServiceTicket()
    {
        Id = 5,
        CustomerId = 24,
        EmployeeId = 87,
        Description = "Description for ticket number 5.",
        Emergency = false,
        DateCompleted = new DateTime(2023, 12, 10),
    }
};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// service tickets
app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticket = serviceTickets.FirstOrDefault(t => t.Id == id);
    if (ticket == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(ticket);
});

// employees
app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(employee);
});

// customers
app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) 
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.Run();

