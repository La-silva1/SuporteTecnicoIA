using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiCadastro.Models;

namespace ApiCadastro.Tests
{
    public class ValidationTests
    {
        private void ValidateModel(object model)
        {
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
        }

        [Fact]
        public void RegisterRequest_ShouldHaveValidationErrors_WhenRequiredFieldsAreMissing()
        {
            var request = new RegisterRequest(); 
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Senha"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nome"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("CEP"));
        }

        [Fact]
        public void LoginRequest_ShouldHaveValidationErrors_WhenRequiredFieldsAreMissing()
        {
            var request = new LoginRequest(); 
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Senha"));
        }

        [Fact]
        public void LoginRequest_ShouldHaveValidationErrors_WhenEmailIsInvalid()
        {
            var request = new LoginRequest { Email = "invalid-email", Senha = "password" };
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
        }

        [Fact]
        public void TicketRequest_ShouldHaveValidationErrors_WhenRequiredFieldsAreMissing()
        {
            var request = new TicketRequest(); 
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Titulo"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Descricao"));
        }

        [Fact]
        public void TicketAvaliacaoRequest_ShouldHaveValidationErrors_WhenNotaIsOutOfRange()
        {
            var request = new TicketAvaliacaoRequest { Nota = 0 }; 
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nota"));

            request = new TicketAvaliacaoRequest { Nota = 6 }; 
            validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nota"));
        }
    }
}
