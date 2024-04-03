using System.Net.Mail;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeOnEventUseCase
{

    private readonly PassInDbContext _dbContext;
    
    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    
    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);
        
        var entity = new Infrastructure.Entities.Attendee
        {
            Name = request.Name,
            Email = request.Email,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);
        if (eventEntity is null)
        {
            throw new NotFoundException("An event with this id does not exist");
        }
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("The user name is invalid");
        }

        var emailIsValid = EmailIsValid(request.Email);
        if (emailIsValid == false)
        {
            throw new ErrorOnValidationException("The user email is invalid");
        }

        var attendeeAlreadyRegistered = _dbContext.Attendees.Any(at => at.Email.Equals(request.Email) && at.Event_Id == eventId);
        if (attendeeAlreadyRegistered)
        {
            throw new ErrorOnValidationException("The user is already registered in that event");
        }

        var attendeesForEvent = _dbContext.Attendees.Count(at => at.Event_Id == eventId);

        if (attendeesForEvent == eventEntity.Maximum_Attendees)
        {
            throw new ErrorOnValidationException("There's no room for this event");
        }
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}