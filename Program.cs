using System.Net.Sockets;
using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

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
    },
    new Employee()
    {
        Id = 90,
        Name = "Michael Scott",
        Specialty = "Who knows"
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
        EmployeeId = null,
        Description = "Description for ticket number 2.",
        Emergency = true,
        DateCompleted = null,
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 33,
        EmployeeId = 77,
        Description = "Description for ticket number 3.",
        Emergency = false,
        DateCompleted = null,
    },
     new ServiceTicket()
    {
        Id = 4,
        CustomerId = 24,
        EmployeeId = null,
        Description = "Description for ticket number 4.",
        Emergency = true,
        DateCompleted = null,
    },
     new ServiceTicket()
    {
        Id = 5,
        CustomerId = 24,
        EmployeeId = 77,
        Description = "Description for ticket number 5.",
        Emergency = false,
        DateCompleted = new DateTime(2023, 12, 15),
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
app.MapGet("/api/servicetickets", () =>
{
    return serviceTickets;
});

// read a single service ticket by 
app.MapGet("/api/servicetickets/{id}", (int id) =>
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
app.MapPost("/api/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

// add a completion date to ticket
app.MapPost("/api/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(s => s.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

// delete a ticket
app.MapDelete("/api/servicetickets/{id}", (int id) =>
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
app.MapPut("/api/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
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

// ********** Employees **********
// get all employees
app.MapGet("/api/employees", () =>
{
    return employees;
});

// get a single employee
app.MapGet("/api/employees/{id}", (int id) =>
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
app.MapGet("/api/customers", () =>
{
    return customers;
});

// get a single customer
app.MapGet("/api/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) 
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

// ********** Extra Endpoints **********

// 1. Emergencies - get all service tickets that are incomplete and emergencies
app.MapGet("/api/servicetickets/emergencies/incomplete", () =>
{
    List<ServiceTicket> emergencies = serviceTickets.Where(s => s.Emergency == true && s.DateCompleted == null).ToList();
    return Results.Ok(emergencies);
});

// 2. Unassigned - return all currently unassigned service tickets

app.MapGet("/api/tickets/unassigned", () =>
{
    List<ServiceTicket> unassigned = serviceTickets.Where(ticket => ticket.EmployeeId == null).ToList();
    return Results.Ok(unassigned);
});

// 3. Inactive Customers - return all of the customers that haven't had a service ticket closed for them in over a year

app.MapGet("/api/customers/inactive", () =>
{
    List<Customer> inactiveCustomers = new();
    List<ServiceTicket> pastTickets = serviceTickets.Where(ticket => ticket.DateCompleted != null && ticket.DateCompleted <= DateTime.Now.AddYears(-1)).ToList();
    List<ServiceTicket> currentTickets = serviceTickets.Where(ticket => ticket.DateCompleted > DateTime.Now.AddYears(-1) || ticket.DateCompleted == null).ToList();
    inactiveCustomers = customers.Where(c => pastTickets.Any(pt => pt.CustomerId == c.Id) && !currentTickets.Any(ct => ct.CustomerId == c.Id)).ToList();
    return Results.Ok(inactiveCustomers);
});


// 4. Available Employees - return employees not currently assigned to an incomplete service ticket
app.MapGet("/api/employees/unassigned", () =>
{
    List<Employee> unassignedEmployees = new();
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(x => x.DateCompleted == null).ToList();
    unassignedEmployees = employees.Where(u => !incompleteTickets.Any(t => t.EmployeeId == u.Id)).ToList();
    return Results.Ok(unassignedEmployees);
});

// 5. Employee's Customers - return all of the customers for whom a given employee has been assigned to a service ticket (whether completed or not)
app.MapGet("/api/employee/{id}/customers", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    List<ServiceTicket> employeeTickets = serviceTickets.Where(x => x.EmployeeId == id).ToList();
    List<Customer> employeeCustomers = customers.Where(x => employeeTickets.Any(t => t.CustomerId == x.Id)).ToList();
    return Results.Ok(employeeCustomers);
});

// 6. Employee of the Month - return the employee who has completed the most service tickets last month.
app.MapGet("/api/employee-of-the-month", () =>
{
    DateTime lastMonth = DateTime.Now.AddMonths(-1);
    Employee employeeOfTheMonth = employees
        .OrderByDescending(e => serviceTickets
            .Count(st => st.EmployeeId == e.Id && st.DateCompleted <= lastMonth))
            .FirstOrDefault();
    return Results.Ok(employeeOfTheMonth);
});

// 7. Past Ticket Review - return completed tickets in order of the completion date, oldest first
app.MapGet("/api/servicetickets/sort/oldest-to-newest", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(d => d.DateCompleted != null).ToList();
    List<ServiceTicket> oldestToNewest = completedTickets.OrderBy(d => d.DateCompleted).ToList();
    return Results.Ok(oldestToNewest);
});
// 8. Prioritized Tickets - return all tickets that are incomplete, in order first by whether they are emergencies, then by whether they are assigned or not (unassigned first)
app.MapGet("/api/servicetickets/priority", () =>
{
    List<ServiceTicket> incompleteTickets = serviceTickets.Where(d => d.DateCompleted == null).ToList();
    List<ServiceTicket> incompleteEmergencies = incompleteTickets.OrderBy(s => s.Emergency != true).ThenBy(e => e.EmployeeId != null).ToList();
    return Results.Ok(incompleteEmergencies);
});

app.Run();

