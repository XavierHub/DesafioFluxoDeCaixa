using EmpXpo.Accounting.Domain.Enumerators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmpXpo.Accounting.WebApp.ViewModels
{
    public class CashFlowEntryViewModel
    {
        [DisplayName("Tipo")]
        [Required(ErrorMessage = "O Campo 'Tipo' é obrigatório")]
        public CashFlowType Type { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Valor")]
        [Required(ErrorMessage = "O Campo 'Valor' é obrigatório")]
        public double Amount { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "O Campo 'Descrição' é obrigatório")]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
