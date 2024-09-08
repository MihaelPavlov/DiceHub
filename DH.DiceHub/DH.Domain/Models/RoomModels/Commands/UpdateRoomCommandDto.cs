﻿using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.RoomModels.Commands
{
    public class UpdateRoomCommandDto : IValidableFields
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int MaxParticipants { get; set; }
        public int GameId { get; set; }

        public bool FieldsAreValid(out List<ValidationError> validationErrors)
        {
            var errors = new List<ValidationError>();

            validationErrors = errors;

            return !validationErrors.Any();
        }

    }
}
