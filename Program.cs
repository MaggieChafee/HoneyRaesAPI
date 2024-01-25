using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Builder;

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
        EmployeeId = null,
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

// ********** Service Tickets **********
// read all service tickets
app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

// read a single service ticket by 
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticket = serviceTickets.FirstOrDefault(t => t.Id == id);
    if (ticket == null)
    {
        return Results.NotFound();
    }
    ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
    return Results.Ok(ticket);
});

// create a service ticket
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

// add a completion date to ticket
app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(s => s.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

// delete a ticket
app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket deleteTicket = serviceTickets.FirstOrDefault(s => s.Id == id);
    if (deleteTicket == null)
    {
        return Results.NotFound();
    }
    serviceTickets.Remove(deleteTicket);
    return Results.Ok();
});

// assign an employee to a service ticket 
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(s => s.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    serviceTickets[ticketIndex].EmployeeId = serviceTicket.EmployeeId;
    return Results.Ok();
});

// get all service tickets that are incomplete and emergencies
app.MapGet("/servicetickets/emergencies/incomplete", () =>
{
    List<ServiceTicket> emergencies = serviceTickets.Where(s => s.Emergency == true && s.DateCompleted == null).ToList();
    return emergencies;
});

// ********** Employees **********
// get all employees
app.MapGet("/employees", () =>
{
    return employees;
});

// get a single employee
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

// ********** Customers **********
// get all customers
app.MapGet("/customers", () =>
{
    return customers;
});

// get a single customer
app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) 
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});



app.Run();

